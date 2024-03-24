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
    }
}
