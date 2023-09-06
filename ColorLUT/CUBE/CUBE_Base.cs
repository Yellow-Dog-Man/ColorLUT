using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ColorLUT.CUBE
{
    public abstract class CUBE_Base
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        protected void IncrementPosition(int dimensions, int size)
        {
            X++;

            if (dimensions == 3)
            {
                if (X == size)
                {
                    X = 0;
                    Y++;
                }

                if (Y == size)
                {
                    Y = 0;
                    Z++;
                }
            }
        }

        protected bool ReachedEnd(int dimensions, int size)
        {
            switch (dimensions)
            {
                case 1: return X == size;
                case 3: return Z == size;

                default:
                    throw new NotImplementedException("Dimensions: " + dimensions);
            }
        }
    }
}
