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
    public class MainMenuScreen
    {
        #region Fields
        private Vector2 _entriesPosition;
        private Vector2 _titlePosition;
        #endregion

        #region Properties
        public List<MenuEntry> MenuEntries { get; private set; }
        public Vector2 EntriesPosition
        {
            get { return _entriesPosition; }
            set { _entriesPosition = value; }
        }

        public Vector2 TitlePosition
        {
            get { return _titlePosition; }
            set { _titlePosition = value; }
        }

        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont GameFont { get; private set; }
        public string MenuTitle { get; private set; }
        public int SelectedEntry { get; private set; }
        public KeyboardState CurrentKeyboardState { get; private set; }
        public KeyboardState PreviousKeyboardState { get; private set; }
        public Texture2D BackgroundTexture { get; private set; }
        public bool IsActive { get; set; }
        #endregion

        public MainMenuScreen(Texture2D backgroundTexture, SpriteBatch spriteBatch, SpriteFont gameFont)
        {
            BackgroundTexture = backgroundTexture;
            SpriteBatch = spriteBatch;
            GameFont = gameFont;
            IsActive = true;

            MenuTitle = "AMERICA THE GAME";
            MenuEntry _playGameMenyEntry = new MenuEntry("Play Game", spriteBatch, gameFont);
            MenuEntry _exitMenuEntry = new MenuEntry("Exit", spriteBatch, gameFont);

            MenuEntries = new List<MenuEntry>();
            MenuEntries.Add(_playGameMenyEntry);
            MenuEntries.Add(_exitMenuEntry);
        }

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                SelectedEntry--;
                if (SelectedEntry < 0)
                    SelectedEntry = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                SelectedEntry++;
                if (SelectedEntry > MenuEntries.Count - 1)
                    SelectedEntry = MenuEntries.Count - 1;
            }
            CurrentKeyboardState = Keyboard.GetState();
            if (!PreviousKeyboardState.Equals(CurrentKeyboardState) && CurrentKeyboardState.IsKeyDown(Keys.Enter))
            {
                switch (SelectedEntry)
                {
                    case 0:
                        IsActive = false;
                        AmericaTheGame.CurrentAmericaTheGame.PlayMusic(AmericaTheGame.CurrentAmericaTheGame.GameplayMusic);
                        break;
                    case 1:
                        AmericaTheGame.CurrentAmericaTheGame.Exit();
                        break;
                }
            }
            PreviousKeyboardState = CurrentKeyboardState;
        }
        public void Draw()
        {
            UpdateMenuEntryPositions();
            AmericaTheGame.CurrentAmericaTheGame.GraphicsDevice.Clear(Color.WhiteSmoke);

            if (BackgroundTexture != null)
            {
                Rectangle _backgroundRect = new Rectangle(0, 0, AmericaTheGame.CurrentAmericaTheGame.GraphicsDevice.Viewport.Width, AmericaTheGame.CurrentAmericaTheGame.GraphicsDevice.Viewport.Height);
                SpriteBatch.Draw(BackgroundTexture, _backgroundRect, Color.WhiteSmoke);
                SpriteBatch.DrawString(GameFont, MenuTitle, TitlePosition, Color.Orange);
            }

            for (int i = 0; i < MenuEntries.Count; i++)
            {
                bool _isSelected = (i == SelectedEntry);
                MenuEntries.ElementAt(i).Draw(_isSelected);
            }

        }
        #endregion

        #region Private Methods
        private void UpdateMenuEntryPositions()
        {
            EntriesPosition = new Vector2(0f, 175f);
            TitlePosition = new Vector2(0f, 50f);

            foreach (MenuEntry menuEntry in MenuEntries)
            {
                _entriesPosition.X = (AmericaTheGame.CurrentAmericaTheGame.GraphicsDevice.Viewport.Width - menuEntry.GetWidth()) / 2;
                _titlePosition.X = (AmericaTheGame.CurrentAmericaTheGame.GraphicsDevice.Viewport.Width - GameFont.MeasureString(MenuTitle).X) / 2;

                menuEntry.Position = EntriesPosition;

                _entriesPosition.Y += menuEntry.GetHeight();
            }
        }
        #endregion
    }
}


