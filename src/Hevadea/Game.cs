﻿using Hevadea.Framework;
using Hevadea.Framework.Threading;
using Hevadea.Framework.Utils;
using Hevadea.Framework.Utils.Json;
using Hevadea.Loading;
using Hevadea.Multiplayer;
using Hevadea.Scenes;
using Hevadea.Scenes.Menus;
using Hevadea.Storage;
using Hevadea.WorldGenerator;
using Hevadea.Worlds;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Hevadea
{
    public class Game
    {
        public static readonly int Unit = 16;
        public static readonly string Name = "Hevadea";
        public static readonly string Version = "0.1.0";
        public static readonly int VersionNumber = 1;

        public static string GetSaveFolder()
        {
            return Rise.Platform.GetStorageFolder() + "/Saves/";
        }

        public static void SetLastGame(string path)
        {
            File.WriteAllText(Rise.Platform.GetStorageFolder() + "/.lastgame", path);
        }

        public static string GetLastGame()
        {
            if (File.Exists(Rise.Platform.GetStorageFolder() + "/.lastgame"))
                return File.ReadAllText(Rise.Platform.GetStorageFolder() + "/.lastgame");
            
            return null;
        }

		public static void Play(string gamePath)
		{
			var job = Jobs.LoadWorld.SetArguments(new Jobs.WorldLoadInfo(gamePath));

            job.Finish += (task, e) 
				=> Rise.Scene.Switch(new SceneGameplay((Game)((Job)task).Result));
			
            Rise.Scene.Switch(new LoadingScene(job));   
		}

		public static void New(string name, Generator generator)
		{
			var job = Jobs.GenerateWorld;
			job.SetArguments(new Jobs.WorldGeneratorInfo($"{GetSaveFolder()}{name}/", Rise.Rnd.NextInt(), generator));

            job.Finish += (s, e) =>
            {
                Game game = (Game)((Job)s).Result;
                game.Initialize();
                Rise.Scene.Switch(new SceneGameplay(game));
            };
            Rise.Scene.Switch(new LoadingScene(job));
		}

        public bool IsClient => this is RemoteGame;
        public bool IsServer => this is HostGame;
        public bool IsLocal => !IsClient && !IsServer;
        public bool IsMaster => IsLocal || IsServer;

        Menu _currentMenu;
        LevelSpriteBatchPool _spriteBatchPool = new LevelSpriteBatchPool();

        public string SavePath { get; set; } = "./test/";

        public Camera Camera { get; set; }
        public World World { get; set; }

        public PlayerSession LocalPlayer { get; set; }
        public List<PlayerSession> Players { get; } = new List<PlayerSession>();

        public Menu CurrentMenu { get => _currentMenu; set { CurrentMenuChange?.Invoke(_currentMenu, value); _currentMenu = value; } }

        public delegate void CurrentMenuChangeHandler(Menu oldMenu, Menu newMenu);

        public event CurrentMenuChangeHandler CurrentMenuChange;

        // --- Initialize, Update and Draw ---------------------------------- //

        public void Initialize()
        {
            Logger.Log<Game>("Initializing...");
            World.Initialize(this);

            if (!Rise.NoGraphic)
                CurrentMenu = new MenuInGame(this);

            if (LocalPlayer != null)
            {
                LocalPlayer.Join(this);
                Camera = new Camera(LocalPlayer.Entity);
                Camera.JumpToFocusEntity();
            }
        }

        public void Draw(GameTime gameTime)
        {
            Camera.FocusEntity.Level.Draw(_spriteBatchPool, gameTime);
        }

        public void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);
            LocalPlayer?.InputHandler?.Update(gameTime);


            if (IsLocal)
            {
                LocalPlayer.Entity.Level.Update(gameTime);
            }
            else if (IsServer)
            {
                foreach (var level in World.Levels)
                {
                    level.Update(gameTime);
                }
            }

            World.DayNightCycle.UpdateTime(gameTime.ElapsedGameTime.TotalSeconds);
        }

        // --- Path generator ----------------------------------------------- //

        public string GetSavePath()
            => $"{SavePath}/";

        public string GetLevelSavePath(Level level)
            => $"{SavePath}/{level.Name}/";

        public string GetLevelMinimapSavePath(Level level)
            => $"{SavePath}/{level.Name}/minimap.png";

        public string GetLevelMinimapDataPath(Level level)
            => $"{SavePath}/{level.Name}/minimap.json";

        // --- Save and load ------------------------------------------------ //

        public Game Load(Job job, string saveFolder)
        {
            SavePath = saveFolder;

            job.Report("Loading world...");

            string path = GetSavePath();

            WorldStorage worldStorage = File.ReadAllText(path + "world.json").FromJson<WorldStorage>();
            World world = World.Load(worldStorage);
            PlayerSession player = PlayerSession.Load(File.ReadAllText(path + "player.json").FromJson<PlayerStorage>());

            foreach (var levelName in worldStorage.Levels)
            {
                world.Levels.Add(LoadLevel(job, this, levelName));
            }

            World = world;
            LocalPlayer = player;

            return this;
        }

        public Level LoadLevel(Job job, Game game, string levelName)
        {
            string levelPath = $"{game.GetSavePath()}{levelName}/";
            Level level = Level.Load(File.ReadAllText(levelPath + "level.json").FromJson<LevelStorage>());

            job.Report($"Loading level {level.Name}...");
            for (int x = 0; x < level.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < level.Chunks.GetLength(1); y++)
                {
                    level.Chunks[x, y] = Chunk.Load(File.ReadAllText(levelPath + $"r{x}-{y}.json").FromJson<ChunkStorage>());
                    job.Report((x * level.Chunks.GetLength(1) + y) / (float)level.Chunks.Length);
                }
            }
            
            if (!Rise.NoGraphic)
            {            
                level.Minimap.Waypoints = File.ReadAllText(levelPath + "minimap.json").FromJson<List<MinimapWaypoint>>();

                var task = new Job((j, args) =>
                {
                    level.Minimap.LoadFromFile(levelPath + "minimap.png");
                    return null;
                });

                Rise.GameLoopThread.Enqueue(task);

                task.Wait();
            }

            return level;
        }

        public void Save(Job job, string savePath)
        {
            SavePath = savePath;

            job.Report("Saving world...");

            var levelsName = new List<string>();

            Directory.CreateDirectory(SavePath);

            foreach (var level in World.Levels)
            {
                SaveLevel(job, level);
            }

            Directory.CreateDirectory(SavePath + "players");

            foreach (var player in Players)
            {
                if (player != LocalPlayer)
                    File.WriteAllText(SavePath + "players/" + player.Token.ToString("x") + ".json", player.Save().ToJson());
            }

            File.WriteAllText(GetSavePath() + "world.json", World.Save().ToJson());
            File.WriteAllText(GetSavePath() + "player.json", LocalPlayer.Save().ToJson());
        }

        void SaveLevel(Job job, Level level)
        {
            job.Report($"Saving {level.Name}...");
            string path = GetLevelSavePath(level);
            Directory.CreateDirectory(path);

            File.WriteAllText(path + "level.json", level.Save().ToJson());

            foreach (var chunk in level.Chunks)
            {
                job.Report((chunk.X * level.Chunks.GetLength(1) + chunk.Y) / (float)level.Chunks.Length);
                File.WriteAllText(path + $"r{chunk.X}-{chunk.Y}.json", chunk.Save().ToJson());
            }

            if (!Rise.NoGraphic) // TODO: Make the minimap store in a bitmap on sever side...
            {
                File.WriteAllText(path + "minimap.json", level.Minimap.Waypoints.ToJson());
                
                var task = new Job((j, args) =>
                {
                    level.Minimap.SaveToFile(path + "minimap.png");
                    return null;
                });

                job.Report($"Saving {level.Name} minimap...");
                job.Report(1f);

                Rise.GameLoopThread.Enqueue(task);

                task.Wait();
            }
        }


    }
}