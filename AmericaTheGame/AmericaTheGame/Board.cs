using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    class Board
    {
        #region Fields
        private Random _rnd = new Random();
        #endregion

        #region Properties
        public Tile[,] Tiles { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public Texture2D TileTexture { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public static Board CurrentBoard { get; private set; }  // Static means it belongs to the class not the object => Note: This means it can be overwritten by a different board object
        public int BoardHeight { get; private set; }
        public int BoardWidth { get; private set; }
        public bool[,] IsTileBlocked { get; set; }
        public int MyProperty { get; set; }
        public Vector2[][] PlatformStartingPositions { get; private set; }
        public int[][] PlatformSizes { get; private set; }
        #endregion

        public Board(SpriteBatch spriteBatch, Texture2D tileTexture, int columns, int rows, int level)
        {
            SpriteBatch = spriteBatch;
            TileTexture = tileTexture;
            Columns = columns;
            Rows = rows;

            InitializeLevel(level);
            SetAllBorderTilesBlocked();
            SetTopLeftTileUnblocked();

            BoardHeight = Tiles.GetLength(1) * Tile.Height;
            BoardWidth = Tiles.GetLength(0) * Tile.Width;
            Board.CurrentBoard = this;  // Save the current board object when it gets initialized
            
        }

        #region Public Methods
        public void CreateNewBoard(int level)
        {
            InitializeLevel(level);
            SetAllBorderTilesBlocked();
            SetTopLeftTileUnblocked();
        }

        public Vector2 WhereCanIGetTo(Vector2 originalPosition, Vector2 destination, Rectangle originalRect, bool platformCheck = false)
        {
            MovementWrapper move = new MovementWrapper(originalPosition, destination, originalRect);
            for (int step = 1; step <= move.NumberOfStepsToBreakMovementInto; step++)
            {
                Vector2 positionToTry = originalPosition + move.OneStep * step;
                Rectangle originalBoundaryRect = CreateRectangleAtPosition(originalPosition, originalRect.Width, originalRect.Height);
                Rectangle nextBoundaryRect = CreateRectangleAtPosition(positionToTry, originalRect.Width, originalRect.Height);
                Rectangle boundaryToCheckRect;

                if (destination.X < originalPosition.X)
                {
                    boundaryToCheckRect = CreateRectangleAtPosition(positionToTry, 1, originalRect.Height);
                }
                else
                {
                    boundaryToCheckRect = CreateRectangleAtPosition(Vector2.Add(positionToTry,Vector2.UnitX*originalRect.Width), 1, originalRect.Height);
                }

                if (HasRoomForRectangle(nextBoundaryRect) && (!platformCheck || CheckPlatformNotEnding(originalBoundaryRect, boundaryToCheckRect)))
                    move.FurthestAvailableLocationSoFar = positionToTry;  
                else
                {
                    if (move.IsDiagonalMove)
                    {
                        move.FurthestAvailableLocationSoFar = CheckPossibleNonDiagonalMovement(move, step, platformCheck);
                    }
                    break;
                }
            }
            return move.FurthestAvailableLocationSoFar;
        }

        public bool CheckPlatformNotEnding(Rectangle originalBoundary, Rectangle rectangleToCheck)
        {
            if (Jumper.IsOnFirmGround(originalBoundary))
            {
                return Jumper.IsOnFirmGround(rectangleToCheck);
            }
            else
                return true;
        }

        public bool HasRoomForRectangle(Rectangle rectangleToCheck)
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.IsBlocked && tile.Bounds.Intersects(rectangleToCheck))
                {
                    return false;
                }
            }
            return true;
        }

        public void Draw()
        {
            foreach (Tile tile in Tiles)
            {
                tile.Draw();
            }
        }
        #endregion

        #region Private Methods
        private void SetPlatformStartingPositions(int[][] posX, int[][] posY, int level)
        {
            PlatformStartingPositions = new Vector2[2][];
            PlatformStartingPositions[0] = new Vector2[6];
            PlatformStartingPositions[1] = new Vector2[6];
            for (int i = 0; i < posX[level].Length; i++)
            {
                PlatformStartingPositions[level][i] = new Vector2(posX[level][i] * Tile.Width, posY[level][i] * Tile.Height);
            }
        }

        private void SetPlatformSizes(int[][] size, int level)
        {
            PlatformSizes = new int[2][];
            PlatformSizes[0] = new int[6];
            PlatformSizes[1] = new int[6];
            for (int i = 0; i < size[level].Length; i++)
            {
                PlatformSizes[level][i] = size[level][i] * Tile.Width;
            }
        }

        private void SetAllTilesUnBlocked()
        {
            for (int j = 0; j < IsTileBlocked.GetLength(1); j++)
            {
                for (int i = 0; i < IsTileBlocked.GetLength(0); i++)
                {
                    IsTileBlocked[i, j] = false;
                }
            }
        }

        private void InitializePlatforms(int level)
        {
            int[][] platformPosX = { new int[6] { 13,  5, 5, 13, 21, 21 }, new int[6] { 13,  5, 5, 13, 21, 21 } };
            int[][] platformPosY = { new int[6] {  4, 13, 7, 10, 13,  7 }, new int[6] {  4, 13, 7, 10, 13,  7 } };
            int[][] platformSize = { new int[6] {  5,  5, 5,  5,  5,  5 }, new int[6] {  4,  4, 4,  4,  4,  4 } };

            SetPlatformStartingPositions(platformPosX, platformPosY, level);
            SetPlatformSizes(platformSize, level);
            SetAllTilesUnBlocked();

            for (int j = 0; j < platformSize[level].Length; j++)
            {
                for (int i = 0; i < platformSize[level][j]; i++)
                {
                    IsTileBlocked[platformPosX[level][j] + i, platformPosY[level][j]] = true;
                }
            }
        }

        private void InitializeLevel(int level)
        {
            IsTileBlocked = new bool[30, 17];
            
            InitializePlatforms(level);

            Tiles = new Tile[Columns, Rows];
            for (int j = 0; j<Rows; j++)
            {
                for (int i = 0; i<Columns; i++)
                {
                    Vector2 tilePosition = new Vector2(i * Tile.Width, j * Tile.Height);
                    Tiles[i, j] = new Tile(TileTexture, tilePosition, SpriteBatch, IsTileBlocked[i, j]);
                }
            }
        }

        private Vector2 CheckPossibleNonDiagonalMovement(MovementWrapper wrapper, int step, bool platformCheck)
        {
                int stepsLeft = wrapper.NumberOfStepsToBreakMovementInto - (step - 1);

                Vector2 remainingHorizontalMovement = wrapper.OneStep.X * Vector2.UnitX * stepsLeft;
                Vector2 finalPositionIfMovingHorizontally = wrapper.FurthestAvailableLocationSoFar + remainingHorizontalMovement;
                wrapper.FurthestAvailableLocationSoFar = WhereCanIGetTo(wrapper.FurthestAvailableLocationSoFar, finalPositionIfMovingHorizontally, wrapper.originalRectangle, platformCheck);

                Vector2 remainingVerticalMovement = wrapper.OneStep.Y * Vector2.UnitY * stepsLeft;
                Vector2 finalPositionIfMovingVertically = wrapper.FurthestAvailableLocationSoFar + remainingVerticalMovement;
                wrapper.FurthestAvailableLocationSoFar = WhereCanIGetTo(wrapper.FurthestAvailableLocationSoFar, finalPositionIfMovingVertically, wrapper.originalRectangle, platformCheck);

            return wrapper.FurthestAvailableLocationSoFar;
        }

        private void SetTopLeftTileUnblocked()
        {
            Tiles[1, 1].IsBlocked = false;
        }

        private Rectangle CreateRectangleAtPosition(Vector2 positionToTry, int width, int height)
        {
            return new Rectangle((int)positionToTry.X, (int)positionToTry.Y, width, height);
        }

        private void InitializeAllTilesAndBlockSomeRandomly()
        {
            Tiles = new Tile[Columns, Rows];
            for (int j = 0; j < Rows; j++)
            {
                for (int i = 0; i < Columns; i++)
                {
                    Vector2 tilePosition = new Vector2(i * TileTexture.Width, j * Tile.Height); //63 = TileTexture.Height
                    Tiles[i, j] = new Tile(TileTexture, tilePosition, SpriteBatch, _rnd.Next(5) == 0);
                }
            }

        }

        private void SetAllBorderTilesBlocked()
        {
            for (int j = 0; j < Rows; j++)
            {
                for (int i = 0; i < Columns; i++)
                {
                    if (j == 0 || i == 0 || j == Rows - 1 || i == Columns - 1)
                        Tiles[i, j].IsBlocked = true;
                }
            }
        }
        #endregion
    }
}
