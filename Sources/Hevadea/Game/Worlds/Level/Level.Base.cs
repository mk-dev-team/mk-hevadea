﻿using Hevadea.Framework.Graphic.Particles;
using System.Collections.Generic;
using Hevadea.GameObjects.Entities;
using Hevadea.GameObjects.Tiles.Renderers;

namespace Hevadea.Game.Worlds
{
    public partial class Level
    {
        private GameManager _game;
        private World _world;
        private List<Entity>[,] _entitiesOnTiles;
        public TileConection[,] CachedTileConnection;
        
        public int Id { get; set; }
        public LevelProperties Properties { get; }
        public string Name { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        public List<Entity> Entities { get; set; }
        public int[] Tiles { get; set; }
        public Dictionary<string, object>[] TilesData { get; set; }
        
        public ParticleSystem ParticleSystem { get; }
        
        public Level(LevelProperties properties, int width, int height)
        {
            Properties = properties;
            Width = width;
            Height = height;
            ParticleSystem = new ParticleSystem();
            
            Tiles = new int[Width * Height];
            TilesData = new Dictionary<string, object>[Width * Height];
            Entities = new List<Entity>();
            _entitiesOnTiles = new List<Entity>[Width, Height];
            CachedTileConnection = new TileConection[Width,Height];
            
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                _entitiesOnTiles[x, y] = new List<Entity>();
                TilesData[x + y * Width] = new Dictionary<string, object>();
            }
        }
    }
}