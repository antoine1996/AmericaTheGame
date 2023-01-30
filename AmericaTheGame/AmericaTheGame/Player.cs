using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public class Player : Jumper
    {
        public Player(Animation animation, Vector2 position, SpriteBatch spritebatch) : base(animation, position)
        {
            
        }

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            CheckKeyboardAndUpdateMovement();
            AffectWithGravity();
            SimulateFriction();
            MoveAsFarAsPossible(gameTime);
            StopMovingIfBlocked();
            if (Animation != null)
            {
                Animation.Position = Position;
                if (IsHalted) Animation.SetRunDirection(Animation.runDirection.Front);
                Animation.Update(gameTime, Position);
            }
        }
        #endregion
    }
}
