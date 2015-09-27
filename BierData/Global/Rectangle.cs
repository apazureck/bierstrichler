using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Global
{
    [Serializable]
    public struct Rectangle
    {
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(double x, double y, double width, double height)
        {
            X = Convert.ToInt32(x);
            Y = Convert.ToInt32(y);
            Width = Convert.ToInt32(width);
            Height = Convert.ToInt32(height);
        }

        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}
