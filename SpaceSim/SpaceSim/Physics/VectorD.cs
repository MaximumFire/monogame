using System;

namespace SpaceSim.Physics
{
    public class VectorD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public VectorD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public VectorD()
        {
            X = 0;
            Y = 0;
        }

        public override string ToString()
        {
            return $"[{this.X}, {this.Y}]";
        }

        public static VectorD operator+(VectorD v1, VectorD v2)
        {
            return new VectorD(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static VectorD operator-(VectorD v1, VectorD v2)
        {
            return new VectorD(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static VectorD operator*(VectorD v1, double d1)
        {
            return new VectorD(v1.X * d1, v1.Y * d1);
        }

        public static VectorD operator/(VectorD v1, double d1)
        {
            return new VectorD(v1.X / d1, v1.Y / d1);
        }

        public double Magnitude()
        {
            return (double)Math.Sqrt(X * X + Y * Y);
        }
    }
}
