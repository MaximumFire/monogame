using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using Flat.Graphics;
using Flat.Input;
using SpaceSim.Physics;
using System;

namespace SpaceSim
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private Sprites sprites;
        private Screen screen;
        private Camera camera;
        private Shapes shapes;

        const double sixtyFps = 1.0 / 60.0;
        private double timeScale = 1000 * 100;
        private double distScale;

        private int camFocus;

        List<Body> bodies;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            this.graphics.ApplyChanges();

            this.sprites = new Sprites(this);
            this.screen = new Screen(this, 1920, 1080);
            this.shapes = new Shapes(this);
            this.camera = new Camera(this.screen);

            this.bodies = new List<Body>();
            this.bodies.Add(new Body(1.989 * Math.Pow(10, 30), new VectorD(0, 0), 15, Color.Yellow));
            this.bodies.Add(new Body(3.301 * Math.Pow(10, 23), new VectorD(-57.9 * Math.Pow(10, 9), 0), new VectorD(0, 47.9 * Math.Pow(10, 3)), 5, Color.OrangeRed));
            this.bodies.Add(new Body(4.867 * Math.Pow(10, 24), new VectorD(108.2 * Math.Pow(10, 9), 0), new VectorD(0, -35.0 * Math.Pow(10, 3)), 7, Color.LightYellow));
            this.bodies.Add(new Body(5.972 * Math.Pow(10, 24), new VectorD(-149.6 * Math.Pow(10, 9), 0), new VectorD(0, 29.8 * Math.Pow(10, 3)), 5, Color.Blue));
            this.bodies.Add(new Body(6.417 * Math.Pow(10, 23), new VectorD(227.9 * Math.Pow(10, 9), 0), new VectorD(0, -24.1 * Math.Pow(10, 3)), 5, Color.Red));
            this.bodies.Add(new Body(1.899 * Math.Pow(10, 27), new VectorD(-778.4 * Math.Pow(10, 9), 0), new VectorD(0, 13.1 * Math.Pow(10, 3)), 12, Color.Salmon));
            this.bodies.Add(new Body(5.685 * Math.Pow(10, 26), new VectorD(1423.6 * Math.Pow(10, 9), 0), new VectorD(0, -9.7 * Math.Pow(10, 3)), 10, Color.LightYellow));
            this.bodies.Add(new Body(8.682 * Math.Pow(10, 25), new VectorD(-2867 * Math.Pow(10, 9), 0), new VectorD(0, 6.8 * Math.Pow(10, 3)), 9, Color.Aquamarine));
            this.bodies.Add(new Body(1.024 * Math.Pow(10, 26), new VectorD(4488.4 * Math.Pow(10, 9), 0), new VectorD(0, -5.4 * Math.Pow(10, 3)), 8, Color.MidnightBlue));
            this.bodies.Add(new Body(1.471 * Math.Pow(10, 22), new VectorD(-5909.6 * Math.Pow(10, 9), 0), new VectorD(0, 4.64 * Math.Pow(10, 3)), 4, Color.DarkSlateBlue));

            // metres per pixel assuming 16 billion km diameter for max orbit
            this.distScale = 16 * Math.Pow(10, 12) / Math.Min(this.screen.Width, this.screen.Height) / 4;

            this.camFocus = 0;

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            FlatKeyboard keyboard = FlatKeyboard.Instance;
            keyboard.Update();

            FlatMouse mouse = FlatMouse.Instance;
            mouse.Update();

            // Handling Input Goes Here

            if (keyboard.IsKeyClicked(Keys.OemTilde))
            {

            }

            if (keyboard.IsKeyClicked(Keys.Up))
            {
                timeScale += 10000;
            }

            if (keyboard.IsKeyClicked(Keys.Down))
            {
                timeScale -= 10000;
            }

            if (keyboard.IsKeyClicked(Keys.F))
            {
                Util.ToggleFullScreen(graphics);
            }

            if (keyboard.IsKeyClicked(Keys.W))
            {
                this.camera.IncZoom();
            }

            if (keyboard.IsKeyClicked(Keys.S))
            {
                this.camera.DecZoom();
            }

            if (keyboard.IsKeyClicked(Keys.Space))
            {
                if (keyboard.IsKeyDown(Keys.LeftShift))
                {
                    this.camFocus--;
                    if (this.camFocus < 0)
                    {
                        this.camFocus = this.bodies.Count - 1;
                    }
                }
                else
                {
                    this.camFocus++;
                    if (this.camFocus >= this.bodies.Count)
                    {
                        this.camFocus = 0;
                    }
                }
            }

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Physics Goes Here

            foreach (Body a in this.bodies)
            {
                a.acceleration = new VectorD();
                foreach (Body b in this.bodies)
                {
                    if (a != b)
                    {
                        VectorD acc = Gravity.Acceleration(a, b);
                        a.acceleration += acc;
                    }
                    if (a == this.bodies[0])
                    {
                        a.acceleration = new VectorD();
                    }
                }
                a.velocity += a.acceleration * (gameTime.ElapsedGameTime.TotalSeconds / sixtyFps) * timeScale;
                a.position += a.velocity * (gameTime.ElapsedGameTime.TotalSeconds / sixtyFps) * timeScale;
                VectorD pos = GetPixelDistanceLog(a);
                a.x = (int)pos.X;
                a.y = (int)pos.Y;
                a.path.Add(new VectorD(a.x, a.y));
                if (a.path.Count > 250)
                {
                    a.path.RemoveAt(0);
                }
            }

            Vector2 newPos = new Vector2((float)this.bodies[this.camFocus].x, (float)this.bodies[this.camFocus].y);
            camera.MoveTo(newPos);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            GraphicsDevice.Clear(Color.Black);
            this.shapes.Begin(this.camera);

            // Drawing Shapes Goes Here

            foreach (Body a in this.bodies)
            {
                for (int i = 0; i < a.path.Count - 1; i++)
                {
                    this.shapes.DrawLine(
                        new Vector2((float)a.path[i].X, (float)a.path[i].Y), 
                        new Vector2((float)a.path[i+1].X, 
                        (float)a.path[i+1].Y), 1f, a.color);
                }
                                               
                VectorD pixelPositions = GetPixelDistanceLog(a);
                float x = (float)pixelPositions.X;
                float y = (float)pixelPositions.Y;

                this.shapes.DrawCircleFill(x, y, a.size, 50, a.color);
            }

            this.shapes.End();
            this.screen.UnSet();
            this.screen.Present(this.sprites);

            base.Draw(gameTime);
        }

        private VectorD GetPixelDistance(Body a)
        {
            double x = a.position.X / distScale;
            double y = a.position.Y / distScale;
            return new VectorD(x, y);
        }

        private static VectorD GetPixelDistanceLog(Body a)
        {
            // if distance / 10^9 is less than e then use 0.15977 * distance / 10^9 * 140
            // else use log10(distance / 10^9) * 140

            double x = (a.position.X < 0) ? -a.position.X : a.position.X;
            double y = (a.position.Y < 0) ? -a.position.Y : a.position.Y;

            x = x / Math.Pow(10, 9);
            y = y / Math.Pow(10, 9);

            double r = Math.Sqrt(x * x + y * y);

            double xRatio = x / r;
            double yRatio = y / r;

            if (r < MathHelper.E)
            {
                r = 0.15977 * 140 * r;
            }
            else
            {
                r = Math.Log10(r + 1) * 140;
            }

            x = r * xRatio;
            y = r * yRatio;

            x = (a.position.X < 0) ? -x : x;
            y = (a.position.Y < 0) ? -y : y;

            x = (a.position.X == 0) ? 0 : x;
            y = (a.position.Y == 0) ? 0 : y;

            return new VectorD(x, y);
        }
    }
}