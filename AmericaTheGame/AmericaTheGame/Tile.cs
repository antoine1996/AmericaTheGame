using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    class Tile : Sprite
    {
        #region Fields
        static public int _tileWidth = 0;
        static public int _tileHeight = 63;
        #endregion

        #region Properties
        public bool IsBlocked { get; set; }
        static public int Width
        {
            get { if (_tileWidth != 0) return _tileWidth; else return AmericaTheGame.CurrentAmericaTheGame.TileTexture.Width; }
            private set { _tileWidth = value; }
        }
        static public int Height
        {
            get { if (_tileHeight != 0) return _tileHeight; else return AmericaTheGame.CurrentAmericaTheGame.TileTexture.Height; }
            private set { _tileHeight = value; }
        }
        #endregion

        public Tile(Texture2D texture, Vector2 position, SpriteBatch spriteBatch, bool isBlocked)
            : base(texture, spriteBatch, position)
        {
            IsBlocked = isBlocked;
            Width = _tileWidth;
            Height = _tileHeight;
        }

        #region Public Methods
        public override void Draw()
        {
            if (IsBlocked)
            {
                Rectangle destinationRect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
                SpriteBatch.Draw(Texture, destinationRect, Color.WhiteSmoke);
            }
        }
        #endregion
    }
}
