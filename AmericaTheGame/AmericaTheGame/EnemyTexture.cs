using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public class EnemyTexture
    {
        #region Properties
        public Texture2D Texture { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public float Scale { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        #endregion

        EnemyTexture(Texture2D texture, int frameWidth, int frameHeight, float scale)
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            Scale = scale;
            Width = (int)(FrameWidth * Scale);
            Height = (int)(FrameHeight * Scale);
        }
    }
}
