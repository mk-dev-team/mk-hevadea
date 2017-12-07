﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WorldOfImagination.Utils;

namespace WorldOfImagination.GameComponent.UI
{
    public class TextInput: Control
    {
        public string Text { get; set; } = "TextBox";
        public string Hint { get; set; } = "Hint";
        public bool Password = false;
        public SpriteFont Font { get; set; }
        
        private bool charAdded = false;
        private char addedChar = ' ';
        private Animation animation = new Animation();


        public TextInput(UiManager ui) : base(ui)
        {
            UI.Game.Window.TextInput += WindowOnTextInput;
            Font = UI.Ress.font_arial;
        }

        private void WindowOnTextInput(object sender, TextInputEventArgs textInputEventArgs)
        {
            var c = textInputEventArgs.Character;
            if (c.ToString() == Environment.NewLine) return;
            if (c == (char)8)
            {
                if (Text.Length > 0)
                {
                    charAdded = false;
                    Text = Text.Remove(Text.Length - 1, 1);
                }
            }
            else
            {
                Text += c;
                addedChar = c;
                charAdded = true;
                animation.Show = true;
                animation.Speed = 0.5f;
                animation.Reset();
            }
        }

        protected override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 textSize;
            float posX;
            
            if (charAdded)
            {
                textSize = Font.MeasureString(Text.Length <= 1 ? Text : Text.Remove(Text.Length - 1 , 1)); 
                var addedCharSize = Font.MeasureString(addedChar.ToString());
                posX = Bound.X + Bound.Width - textSize.X - addedCharSize.X * (animation.TwoPhases) - 8f;
            }
            else
            {
                textSize = Font.MeasureString(Text); 
                posX = Bound.X + Bound.Width - textSize.X - 8f;
            }
            
            var posY = Bound.Y + Bound.Height / 2 - Font.MeasureString("O").Y / 2;
            
            spriteBatch.FillRectangle(Bound, new Color(0, 0, 0, 200));
            spriteBatch.FillRectangle(new Rectangle((int)(Bound.X + Bound.Width - Bound.Width  / 16f * animation.TwoPhases), Bound.Y,
                                                    Bound.Width, Bound.Height), Color.Gold * ((1f - animation.TwoPhases) * 0.75f));
            
            spriteBatch.DrawString(Font, Hint, new Vector2(Bound.X + 16f, posY), Color.White * 0.45f);
            
            if (Password)
            {
                var height = (int)Font.MeasureString("azertyuiopqsdfghjklmwxcvbn").Y;
                textSize = new Vector2(16 * Text.Length, height);
                posX = Bound.X + Bound.Width - textSize.X - 16 * (animation.TwoPhases);
                
                for (int i = 0; i < Text.Length; i++)
                {
                    spriteBatch.FillRectangle(new Rectangle((int)posX + 16 * i, (int)posY, 14, height),  Color.White);
                }
            }
            else
            { 
                spriteBatch.DrawString(Font, Text, new Vector2(posX, posY), Color.White);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            animation.Update(gameTime);

            if (animation.TwoPhases == 1f)
            {
                charAdded = false;
            }
        }
    }
}
