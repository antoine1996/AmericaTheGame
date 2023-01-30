using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public class Objective : Sprite
    {
        #region Properties
        public bool PreviousStateIsCollision { get; set; }
        public Vector2 CenterPosition { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        #endregion

        public Objective(Texture2D texture, SpriteBatch spriteBatch, float scale, int level) : base(texture, spriteBatch, Vector2.One, scale)
        {
            Scale = scale;
            Width = (int)(Texture.Width * Scale);
            Height = (int)(Texture.Height * Scale);
            SetPosition(level);
            PreviousStateIsCollision = false;
            //Position = Board.CurrentBoard.PlatformStartingPositions[level][0] + new Vector2((Board.CurrentBoard.PlatformSizes[level][0] - (texture.Width * Scale)) / 2, - (texture.Height * Scale));
            //Position.Y -= texture.Height;
        }

        #region Public Methods
        public void Update(Player player, Animation explosionAninamtion, int level)
        {
            Collision(player, explosionAninamtion);
            LoadNextLevelOrEndGame(explosionAninamtion, level);
        }

        public override void Draw()
        {
            Rectangle destinationRect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            SpriteBatch.Draw(Texture, destinationRect, Color.WhiteSmoke);
        }

        public void SetPosition(int level)
        {
            Position = Board.CurrentBoard.PlatformStartingPositions[level][0] + new Vector2((Board.CurrentBoard.PlatformSizes[level][0] - (Texture.Width * Scale)) / 2, -(Texture.Height * Scale));
        }
        #endregion

        #region Private Methods
        private void Collision(Player player, Animation explosionAnimation)
        {
            if (this.Bounds.Intersects(player.Bounds) && explosionAnimation == null)
            {
                AmericaTheGame.CurrentAmericaTheGame.AddExplosion(CenterPosition);
                AmericaTheGame.CurrentAmericaTheGame.ExplosionSound.Play();
                PreviousStateIsCollision = true;
            }
            else if (this.Bounds.Intersects(player.Bounds) && explosionAnimation.Active == false && PreviousStateIsCollision == false)
            {
                explosionAnimation.Active = true;
                AmericaTheGame.CurrentAmericaTheGame.ExplosionSound.Play();
                PreviousStateIsCollision = true;
            }
            else if (!this.Bounds.Intersects(player.Bounds))
            {
                PreviousStateIsCollision = false;
            }
        }

        private void LoadNextLevelOrEndGame(Animation explosionAnimation, int level)
        {
            if (explosionAnimation != null && explosionAnimation.Active == false)
            {
                if (level >= Board.CurrentBoard.PlatformStartingPositions.Length - 1)
                {
                    //AmericaTheGame.CurrentAmericaTheGame.Exit();
                    AmericaTheGame.CurrentAmericaTheGame.NextLevel();
                    AmericaTheGame.CurrentAmericaTheGame.MainMenu.IsActive = true;
                    AmericaTheGame.CurrentAmericaTheGame.PauseMusic();
                }
                else
                    AmericaTheGame.CurrentAmericaTheGame.NextLevel();

                AmericaTheGame.CurrentAmericaTheGame.EmptyExplosionAnimation();                
            }
        }
        #endregion
    }
}
