using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public struct TrumpSpriteStrip
    {
        #region Properties
        public Texture2D Texture { get; private set; }
        public int OffSetPosX { get; private set; }
        public int OffSetPosY { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public static TrumpSpriteStrip CurrentTrumpSpriteStrip { get; private set; }
        #endregion

        public TrumpSpriteStrip(Texture2D texture)
        {
            Texture = texture;
            OffSetPosX = 33;
            OffSetPosY = 12;
            FrameWidth = 60;
            FrameHeight = 76;
            CurrentTrumpSpriteStrip = this;
        }
    }
}
