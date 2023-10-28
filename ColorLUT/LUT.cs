using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLUT
{
    public class LUT
    {
        public string Title { get; set; }

        public int Dimensions { get; private set; }

        int[] size;
        float[] values;

        public LUT(int dimensions, params int[] size)
        {
            if (dimensions != size.Length)
                throw new ArgumentException("Sizes array must match the number of dimensions");
        }
    }
}
