using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceSim.Physics
{
    public class Body
    {
        public double mass;
        public VectorD velocity;
        public VectorD acceleration;
        public Vector2 position;
        public Color color;
        public float size;

        public Body(double mass, Vector2 position, float size, Color color)
        {
            this.mass = mass;
            this.velocity = new VectorD(0, 0);
            this.acceleration = new VectorD(0, 0);
            this.position = position;
            this.color = color;
            this.size = size;
        }
    }
}
