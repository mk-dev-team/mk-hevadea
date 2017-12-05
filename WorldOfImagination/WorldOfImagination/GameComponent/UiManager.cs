﻿using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace WorldOfImagination.GameComponent
{
    public class UiManager : GameComponent
    {
        private SpriteBatch uiSpriteBatch;
        
        public InputManager Input => Game.Input;
        public Ressources Ress => Game.Ress;
        public bool Debug => Game.Debug.Visible;
        
        public UiManager(WorldOfImaginationGame game) : base(game)
        {
            
        }

        public override void Initialize()
        {
            uiSpriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public void RefreshLayout()
        {
            Game.Scene?.CurrentScene?.UiRoot.RefreshLayout();
        }

        public override void Update(GameTime gameTime)
        {
            Game.Scene?.CurrentScene?.UiRoot.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            uiSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp);
            Game.Scene?.CurrentScene?.UiRoot.Draw(uiSpriteBatch, gameTime);
            uiSpriteBatch.End();
        }
    }
}