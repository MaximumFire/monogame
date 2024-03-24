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
        public VectorD position;
        public Color color;
        public float size;

        public Body(double mass, VectorD position, float size, Color color)
        {
            this.mass = mass;
            this.velocity = new VectorD(0, 0);
            this.acceleration = new VectorD(0, 0);
            this.position = position;
            this.color = color;
            this.size = size;
        }

        public Body(double mass, VectorD position, VectorD velocity, float size, Color color)
        {
            this.mass = mass;
            this.velocity = velocity;
            this.acceleration = new VectorD(0, 0);
            this.position = position;
            this.color = color;
            this.size = size;
        }
    }
}
