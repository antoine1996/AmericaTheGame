using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public class Enemy : Jumper
    {
        #region Fields
        int _walkingTime = 2000;
        #endregion

        #region Properties
        public int WalkingTime { get; private set; }
        public float Speed { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        #endregion
        public Enemy(Texture2D texture, Vector2 position, SpriteBatch spritebatch, float speed = 0.4f)
            : base(texture, position, spritebatch)
        {
            WalkingTime = _walkingTime;
            Speed = speed;
            Movement += Vector2.UnitX * Speed;
            Width = texture.Width;
            Height = texture.Height;

            AmericaTheGame.CurrentAmericaTheGame.Enemies.Add(this);
        }

        //public Enemy(Animation animation, Vector2 position, float speed = 0.4f)
        //    : base(animation, position)
        //{
        //    WalkingTime = _walkingTime;
        //    Speed = speed;
        //    Movement += Vector2.UnitX * Speed;
        //    Width = animation.Width;
        //    Height = animation.Height;

        //    AmericaTheGame.CurrentAmericaTheGame.Enemies.Add(this);
        //}

        public void Update(GameTime gameTime, Player player)
        {   
            Movement += Vector2.UnitX * Speed;
            AffectWithGravity();
            SimulateFriction();
            MoveAsFarAsPossible(gameTime, true);
            StopMovingIfBlocked();
            ChangeRunningDirectionIfBlocked();
            if (Animation != null)
            {
                Animation.Position = Position;
                if (IsHalted) Animation.SetRunDirection(Animation.runDirection.Front);
                Animation.Update(gameTime, Position);
            }
            Collision(player);
        }

        #region Private Fields
        private void Collision(Player player)
        {
            if (this.Bounds.Intersects(player.Bounds))
            {
                AmericaTheGame.CurrentAmericaTheGame.ShouldRestartGame = true;
            }
        }
        #endregion
        #region Private Methods
        private void ChangeRunningDirectionIfBlocked()
        {
            if (Movement.X == 0)
            {
                Speed *= -1;
            }
        }
        #endregion
    }
}
