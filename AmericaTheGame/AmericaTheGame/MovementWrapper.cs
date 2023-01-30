using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericaTheGame
{
    public struct MovementWrapper
    {
        #region Properties
        public Vector2 MovementToTry { get; private set; }
        public Vector2 FurthestAvailableLocationSoFar { get; set; }
        public int NumberOfStepsToBreakMovementInto { get; private set; }
        public bool IsDiagonalMove { get; private set; }
        public Vector2 OneStep { get; private set; }
        public Rectangle originalRectangle { get; set; }
        #endregion

        public MovementWrapper(Vector2 originalPosition, Vector2 destination, Rectangle originalRectangle)
        {
            MovementToTry = destination - originalPosition;
            FurthestAvailableLocationSoFar = originalPosition;
            NumberOfStepsToBreakMovementInto = (int)(MovementToTry.Length() * 2) + 1;
            IsDiagonalMove = MovementToTry.X != 0 && MovementToTry.Y != 0;
            OneStep = MovementToTry / NumberOfStepsToBreakMovementInto;
            this.originalRectangle = originalRectangle;
        }
    }
}
