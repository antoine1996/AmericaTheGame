using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public class MenuEntry
    {
        #region Properties
        public string Text { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont GameFont { get; private set; }
        public Vector2 Position { get; set; }
        #endregion

        public MenuEntry(string text, SpriteBatch spriteBatch, SpriteFont gameFont)
        {
            Text = text;
            SpriteBatch = spriteBatch;
            GameFont = gameFont;
        }

        #region Public Methods
        public int GetHeight()
        {
            return (int)GameFont.LineSpacing;
        }

        public int GetWidth()
        {
            return (int)GameFont.MeasureString(Text).X;
        }

        public void Draw(bool isSelected)
        {
            if (isSelected)
                SpriteBatch.DrawString(GameFont, Text, Position, Color.White);
            else
                SpriteBatch.DrawString(GameFont, Text, Position, Color.Orange);
        }

        internal void Draw(object isSelected)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
