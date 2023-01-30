using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public class Explosion : Animation
    {
        #region Fields
        private static bool _isActivated = false ;
        #endregion

        public static bool IsActivated
        {
            get { return _isActivated; }
            private set { _isActivated = value; }
        }

        public Explosion(Texture2D spriteStrip, SpriteBatch spriteBatch, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frameTime, Color color, float scale, bool animationLoop)
            : base(spriteStrip, spriteBatch, position, frameWidth, frameHeight, frameCount,frameTime, color, scale, animationLoop)
        {
            IsActivated = true;
        }
    }
}
