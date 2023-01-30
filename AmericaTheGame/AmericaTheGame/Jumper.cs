using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public class Jumper : Sprite
    {
        #region Fields
        private Vector2 oldPosition;
        private KeyboardState keyboardOldState;
        #endregion

        #region Properties
        public Vector2 Movement { get; set; }
        public static Jumper Trump { get; private set; }
        public bool IsHalted { get; private set; }
        #endregion

        public Jumper(Texture2D texture, Vector2 position, SpriteBatch spriteBatch) : base(texture, spriteBatch, position)
        { }

        public Jumper(Animation animation, Vector2 position) : base(animation, position)
        {
            Trump = this;
        }

        #region Public Methods
        public static bool IsOnFirmGround(Rectangle bounds)
        {
            Rectangle onePixelLower = bounds;
            onePixelLower.Offset(0, 1);
            return !Board.CurrentBoard.HasRoomForRectangle(onePixelLower);
        }
        #endregion

        #region Private/Protected Methods
        protected void StopMovingIfBlocked()
        {
            Vector2 lastMovement = Position - oldPosition;
            if (lastMovement.X == 0) { Movement *= Vector2.UnitY; }
            if (lastMovement.Y == 0) { Movement *= Vector2.UnitX;  }
        }

        protected void AffectWithGravity()
        {
            Movement += Vector2.UnitY * .65f;
        }

        protected void MoveAsFarAsPossible(GameTime gameTime, bool platformCheck = false)
        {
            oldPosition = Position;
            UpdatePositionBasedOnMovement(gameTime);
            Position = Board.CurrentBoard.WhereCanIGetTo(oldPosition, Position, Bounds, platformCheck);
        }

        protected void CheckKeyboardAndUpdateMovement()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (!keyboardState.IsKeyDown(Keys.Left) && !keyboardState.IsKeyDown(Keys.Right))
                IsHalted = true;
            else
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    Movement -= Vector2.UnitX * 0.8f;
                    IsHalted = false;
                    Animation.SetRunDirection(Animation.runDirection.Left);
                }

                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    Movement += Vector2.UnitX * 0.8f;
                    IsHalted = false;
                    Animation.SetRunDirection(Animation.runDirection.Right);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Up) && IsOnFirmGround(this.Bounds))
            {
                if (!keyboardOldState.IsKeyDown(Keys.Up))
                {
                    Movement = -Vector2.UnitY * 20;
                }
            }
            else if (keyboardOldState.IsKeyDown(Keys.Up))
            {
                // Key was down last update, but not now, so it has just been released
            }
            keyboardOldState = keyboardState;
        }

        protected void SimulateFriction()
        {
            if (IsOnFirmGround(this.Bounds)) { Movement *= Vector2.One * .92f; }    // Slow down jumper 92% per update cycle
            else { Movement *= Vector2.One * 0.98f; }                               // Slow down jumper 98% per update cycle
        }

        private void UpdatePositionBasedOnMovement(GameTime gameTime)
        {
                Position += Movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 15;  // Elapsed time since last update
        }
        #endregion
    }
}
