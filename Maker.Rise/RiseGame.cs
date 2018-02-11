﻿using Maker.Rise.Enums;
using Maker.Utils;
using Maker.Utils.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Maker.Rise
{
    public sealed class InternalGame : Game
    {
        public int DrawTime { get; private set; } = 0;
        public int UpdateTime { get; private set; } = 0;

        private readonly Stopwatch drawStopwatch;
        private readonly Stopwatch updateStopwatch;

        public delegate void OnLoadHandler(InternalGame sender, EventArgs e);

        public event OnLoadHandler OnLoad;

        public InternalGame()
        {
            drawStopwatch = new Stopwatch();
            updateStopwatch = new Stopwatch();

            Content.RootDirectory = "Content";
        }

        private static void IntializeGameEngine()
        {
            Engine.Ressource.Initialize();
            EngineRessources.Load();

            Engine.Audio.Initialize();
            Engine.Input.Initialize();
            Engine.Network.Initialize();
            Engine.Scene.Initialize();

            Engine.Ui.Initialize();
            Engine.Debug.Initialize();
        }

        private static void DrawGameComponent(GameTime gameTime)
        {
            Engine.Scene.Draw(gameTime);
            Engine.Debug.Draw(gameTime);
        }

        private static void UpdateGameComponent(GameTime gameTime)
        {
            Engine.Audio.Update(gameTime);
            Engine.Input.Update(gameTime);
            Engine.Network.Update(gameTime);
            Engine.Scene.Update(gameTime);
            Engine.Ressource.Update(gameTime);
            Engine.Debug.Update(gameTime);
        }

        private bool WindowSizeIsBeingChanged = false;
        protected override void Initialize()
        {
            IntializeGameEngine();
            OnLoad?.Invoke(this, new EventArgs());
            Logger.Log<InternalGame>(LoggerLevel.Fine, "Game engine initialized, entering game loop...");
            Window.AllowUserResizing = true;
            
            Window.ClientSizeChanged += (sender, args) =>
            {
                WindowSizeIsBeingChanged = !WindowSizeIsBeingChanged;
                if(WindowSizeIsBeingChanged){
                    Engine.Graphic.g.PreferredBackBufferWidth = Window.ClientBounds.Width;
                    Engine.Graphic.g.PreferredBackBufferHeight = Window.ClientBounds.Height;
                    Engine.Graphic.g.ApplyChanges();
                    Engine.Graphic.ResetRenderTargets();
                }
            };
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            updateStopwatch.Start();
            UpdateGameComponent(gameTime);
            updateStopwatch.Stop();

            UpdateTime = updateStopwatch.Elapsed.Milliseconds;
            updateStopwatch.Reset();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            drawStopwatch.Start();
            DrawGameComponent(gameTime);
            drawStopwatch.Stop();

            DrawTime = drawStopwatch.Elapsed.Milliseconds;
            drawStopwatch.Reset();
        }
    }
}