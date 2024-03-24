using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceSim.Physics
{
    public class Gravity
    {
        public static readonly double gConstant = 6.67 * MathF.Pow(10, -11);
        
        public static VectorD GravitationalForce(Body a, Body b)
        {
            VectorD r = Displacement(a.position, b.position);
            double R = (double)Vector2.Distance(a.position, b.position);
            double F = (Gravity.gConstant * (double)a.mass * (double)b.mass) / (Math.Pow(R, 2));
            double Fx = F * r.X / R;
            double Fy = F * r.Y / R;

            Console.WriteLine((a.mass * b.mass));
            Console.WriteLine((Math.Pow(R, 2)));

            return new VectorD(Fx, Fy);
        }

        private static VectorD Displacement(Vector2 a, Vector2 b)
        {
            return new VectorD((double)(b.X - a.X), (double)(b.Y - a.Y));
        }
    }
}
