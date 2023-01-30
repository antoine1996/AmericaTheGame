using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace AmericaTheGame
{
    public class AmericaTheGame : Game
    {
        #region Fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _tileTexture, _playerTexture, _enemyTexture, _objectiveTexture, _explosionTexture;
        private Player _player;
        private Board _board;
        private SpriteFont _debugFont;
        private Animation _trumpAnimation, _explosionAnimation;
        private TrumpSpriteStrip _trumpSpriteStrip;
        private Objective _objective;
        private int _level;
        private Vector2 _explosionPosition;
        #endregion

        #region Properties
        public static AmericaTheGame CurrentAmericaTheGame { get; set; }
        public Texture2D TileTexture { get; set; }
        public List<Enemy> Enemies { get; set; }
        //public List<Enemy> Enemies;
        public bool ShouldRestartGame { get; set; }
        public SoundEffect ExplosionSound { get; private set; }
        public Texture2D BackGroundTexture1 { get; private set; }
        public Texture2D BackGroundTexture2 { get; private set; }
        public Texture2D MenuBackGroundTexture { get; private set; }
        public Texture2D[] BackGrounds { get; private set; }
        public SpriteFont GameFont { get; private set; }
        public MainMenuScreen MainMenu { get; private set; }
        public Song GameplayMusic { get; set; }
        public bool MusicPaused { get; set; }
        #endregion

        public AmericaTheGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = true;
            CurrentAmericaTheGame = this;
            Enemies = new List<Enemy>();
            Content.RootDirectory = "Content";
            ShouldRestartGame = false;
            MusicPaused = false;
            _level = 0;
        }

        #region Main Methods
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tileTexture = Content.Load<Texture2D>("Graphics/Tile3");
            TileTexture = _tileTexture;
            _playerTexture = Content.Load<Texture2D>("Graphics/TrumpAnimation");
            _enemyTexture = Content.Load<Texture2D>("Graphics/Kim2");
            _objectiveTexture = Content.Load<Texture2D>("Graphics/Nuke2");
            _explosionTexture = Content.Load<Texture2D>("Graphics/Explosion");
            BackGroundTexture1 = Content.Load<Texture2D>("Graphics/BackGround2");
            BackGroundTexture2 = Content.Load<Texture2D>("Graphics/BackGround");
            BackGrounds = new Texture2D[] { BackGroundTexture1, BackGroundTexture2 };
            GameFont = Content.Load<SpriteFont>("SpriteFont/GameFont");
            MenuBackGroundTexture = Content.Load<Texture2D>("Graphics/BackGround3");
            MainMenu = new MainMenuScreen(MenuBackGroundTexture, _spriteBatch, GameFont);

            ExplosionSound = Content.Load<SoundEffect>("Sound/Explosion");

            _trumpSpriteStrip = new TrumpSpriteStrip(_playerTexture);
            _trumpAnimation = new TrumpAnimation(_playerTexture, _spriteBatch, new Vector2(70f, 500f), 60, 76, 6, 90, Color.WhiteSmoke, 1.4f, true, 4, new int[] { 20, 33, 20, 6 }, 12);
            
            _board = new Board(_spriteBatch, _tileTexture, 30, 17, _level);

            _player = new Player(_trumpAnimation, _trumpAnimation.Position, _spriteBatch);
            CreateEnemies(_level, _enemyTexture, _spriteBatch);

            _objective = new Objective(_objectiveTexture, _spriteBatch, 0.12f, _level);
            
            _debugFont = Content.Load<SpriteFont>("SpriteFont/DebugFont");
            
            GameplayMusic = Content.Load<Song>("Sound/StarSpangledBanner8Bit");
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (ShouldRestartGame) RestartGame();
            if(MainMenu.IsActive == true)
                MainMenu.Update(gameTime);
            else
            {
                _player.Update(gameTime);
                UpdateEnemies(gameTime, _player);
                _objective.Update(_player, _explosionAnimation, _level);
                UpdateExplosion(gameTime);
                CheckKeyboardAndReact();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);
            _spriteBatch.Begin();
            if (MainMenu.IsActive == true)
                MainMenu.Draw();
            else
            {
                DrawBackground(_board, _level);
                _board.Draw();
                fillScreen();
                //WriteDebugInformation();
                _player.Draw();
                DrawEnemies();
                _objective.Draw();
                DrawExplosion();
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #region Public Methods
        public void PauseMusic()
        {
            try
            {
                MediaPlayer.Pause();
            }
            catch { }
            MusicPaused = true;
        }

        public void PlayMusic(Song song)
        {
            if (MusicPaused)
            {
                try
                {
                    MediaPlayer.Resume();
                }
                catch { }
            }
            else
            {
                try
                {
                    MediaPlayer.Play(song);
                    MediaPlayer.IsRepeating = true;
                }
                catch { }
            }
        }

        public void EmptyExplosionAnimation()
        {
            _explosionAnimation = null;
        }

        public void AddExplosion(Vector2 position)
        {
            int frameWidth = 134;
            int frameHeight = 134;
            float _scale = 3f;
            _explosionPosition = new Vector2(_objective.Position.X + ((_objective.Width - (int)(frameWidth * _scale)) / 2)
                , _objective.Position.Y + ((_objective.Height - (int)(frameHeight * _scale)) / 2));

            _explosionAnimation = new Animation(_explosionTexture, _spriteBatch, _explosionPosition, frameWidth, frameHeight, 12, 45, Color.WhiteSmoke, _scale, false);
        }

        public void RestartGame()
        {
            ShouldRestartGame = false;
            _level = 0;
            InitializeLevel(_level);
        }

        public void NextLevel()
        {
            if (_level < Board.CurrentBoard.PlatformStartingPositions.Length - 1)
            {
                _level++;
                InitializeLevel(_level);
            }
            else RestartGame();
        }
        #endregion

        #region Private Methods
        private void DrawBackground(Board board, int level)
        {
            int rectWidth = board.BoardWidth;
            int rectHeight = board.BoardHeight;
            Rectangle destinationRect = new Rectangle(0, 0, rectWidth, rectHeight);
            _spriteBatch.Draw(BackGrounds[level], destinationRect, Color.WhiteSmoke);
        }

        private void UpdateExplosion(GameTime gameTime)
        {
            if (_explosionAnimation != null)
                _explosionAnimation.Update(gameTime, _explosionPosition);
        }

        private void DrawExplosion()
        {
            if (_explosionAnimation != null)
                _explosionAnimation.Draw();
        }

        private void DrawEnemies()
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw();
            }
        }

        private void CreateEnemies(int level, Texture2D enemyTexture, SpriteBatch spriteBatch)
        {
            Vector2[] _enemyPositions = new Vector2[_board.PlatformStartingPositions[level].Length-1];
            float _enemySpeed;
            Random rnd = new Random();

            for (int i = 0; i < _board.PlatformStartingPositions[level].Length-1; i++)
            {
                _enemyPositions[i] = Board.CurrentBoard.PlatformStartingPositions[level][i+1];
                _enemyPositions[i].X += rnd.Next(0, Board.CurrentBoard.PlatformSizes[level][i+1] - enemyTexture.Width + 1 );
                _enemyPositions[i].Y -= enemyTexture.Height;

                _enemySpeed = (rnd.Next(0, 9) * ((0.4f - 0.2f) / 9)) + 0.2f;

                //_enemyAnimation = new Animation(_enemyTexture, _spriteBatch, _enemyPositions[i], 175, 241, 1, 45, Color.WhiteSmoke, 1f, true);

                new Enemy(_enemyTexture, _enemyPositions[i], _spriteBatch, _enemySpeed);
            }
        }

        private void RemoveEnemies()
        {
            Enemies.Clear();
        }

        private void UpdateEnemies(GameTime gameTime, Player player)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Update(gameTime, player);
            }
        }

        private void fillScreen()
        {
            int rectWidth = _graphics.GraphicsDevice.Viewport.Width;
            int rectHeight = _graphics.GraphicsDevice.Viewport.Height - Board.CurrentBoard.BoardHeight;
            Texture2D rect = new Texture2D(_graphics.GraphicsDevice, rectWidth, rectHeight);

            Color[] data = new Color[rectWidth * rectHeight];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Black;
            rect.SetData(data);

            Vector2 position = Vector2.UnitY * (Board.CurrentBoard.BoardHeight);
            _spriteBatch.Draw(rect, position, Color.WhiteSmoke);
        }

        private void SetPlayerStartPosition()
        {
            _player.Position = new Vector2(70f, 500f);
            _player.Movement = Vector2.Zero;

        }

        private void InitializeLevel(int level)
        {
            RemoveEnemies();
            Board.CurrentBoard.CreateNewBoard(level);
            SetPlayerStartPosition();
            CreateEnemies(level, _enemyTexture, _spriteBatch);
            _objective.SetPosition(level);
        }

        private void CheckKeyboardAndReact()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.R)) {
                ShouldRestartGame = true; }
            if (state.IsKeyDown(Keys.Escape))
            {
                MainMenu.IsActive = true;
                PauseMusic();
            }
        }

        private void WriteDebugInformation()
        {
            string positionInText = string.Format("Position of Jumper: ({0:0.0}, {1:0.0})", _player.Position.X, _player.Position.Y);
            string movementInText = string.Format("Current movement: ({0:0.0}, {1:0.0})", _player.Movement.X, _player.Movement.Y);
            string isOnFirmGroundText = string.Format("On firm ground? : {0}", Jumper.IsOnFirmGround(_player.Bounds));

            _spriteBatch.DrawString(_debugFont, positionInText, new Vector2(10, 0), Color.Blue);
            _spriteBatch.DrawString(_debugFont, movementInText, new Vector2(10, 20), Color.Blue);
            _spriteBatch.DrawString(_debugFont, isOnFirmGroundText, new Vector2(10, 40), Color.Blue);
        }
        #endregion
    }
}


// Tutorials used as reference for this game:
// http://xbox.create.msdn.com/en-US/education/tutorial/2dgame/getting_started
// http://xnafan.net/2013/04/simple-platformer-game-in-xna-tutorial-part-one/