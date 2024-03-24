using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceSim.Physics
{
    public class Gravity
    {
        public static readonly double gConstant = 6.67 * MathF.Pow(10, -11);
        
        private static VectorD GravitationalForce(Body a, Body b)
        {
            VectorD r = b.position - a.position;
            double R = r.Magnitude();
            double F = (Gravity.gConstant * a.mass * b.mass) / (Math.Pow(R, 2));
            double Fx = F * r.X / R;
            double Fy = F * r.Y / R;
            return new VectorD(Fx, Fy);
        }

        public static VectorD Acceleration(Body a, Body b)
        {
            VectorD g = GravitationalForce(a, b) / a.mass;
            return g;
        }
    }
}
