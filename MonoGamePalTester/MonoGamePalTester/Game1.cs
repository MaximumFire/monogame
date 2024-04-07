using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using Flat.Graphics;
using Flat.Input;
using System;

namespace MonoGamePalTester
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Sprites sprites;
        private Texture2D texture;
        private Screen screen;
        private Camera camera;

        private Shapes shapes;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.texture = this.Content.Load<Texture2D>("circle");
        }

        protected override void Update(GameTime gameTime)
        {
            FlatKeyboard keyboard = FlatKeyboard.Instance;
            keyboard.Update();

            FlatMouse mouse = FlatMouse.Instance;
            mouse.Update();

            if (keyboard.IsKeyClicked(Keys.OemTilde))
            {
                this.camera.GetExtents(out Vector2 min, out Vector2 max);
                Console.WriteLine("CamMin: " + min);
                Console.WriteLine("CamMax: " + max);
            }

            if (keyboard.IsKeyClicked(Keys.F))
            {
                Util.ToggleFullScreen(graphics);
            }

            if (keyboard.IsKeyDown(Keys.Left))
            {
                this.camera.Move(new Vector2(-1, 0));
            }

            if (keyboard.IsKeyDown(Keys.Right))
            {
                this.camera.Move(new Vector2(1, 0));
            }

            if (keyboard.IsKeyDown(Keys.Up))
            {
                this.camera.Move(new Vector2(0, 1));
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                this.camera.Move(new Vector2(0, -1));
            }

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (keyboard.IsKeyClicked(Keys.W))
            {
                this.camera.IncZoom();
            }

            if (keyboard.IsKeyClicked(Keys.S))
            {
                this.camera.DecZoom();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Viewport vp = this.GraphicsDevice.Viewport;
            this.sprites.Begin(this.camera, false);
            //this.sprites.Draw(texture, null, new Vector2(8, 8), new Vector2(0, 0), MathHelper.TwoPi / 4f, new Vector2(0.125f, 0.125f), Color.Yellow);
            this.sprites.End();

            this.shapes.Begin(this.camera);

            Vector2[] vertices = new Vector2[5];
            vertices[0] = new Vector2(0, 2);
            vertices[1] = new Vector2(2, -2);
            vertices[2] = new Vector2(1, -1);
            vertices[3] = new Vector2(-1, -1);
            vertices[4] = new Vector2(-2, -2);

            for (int i = 0; i < 5; i++)
            {
                vertices[i].X *= 50;
                vertices[i].Y *= 50;
            }

            Matrix transform = Matrix.CreateScale(1f) * Matrix.CreateRotationZ(MathHelper.TwoPi / 10f) * Matrix.CreateTranslation(0f, 0f, 0f);

            int[] triangleIndices = new int[9];
            triangleIndices[0] = 0;
            triangleIndices[1] = 1;
            triangleIndices[2] = 2;
            triangleIndices[3] = 0;
            triangleIndices[4] = 2;
            triangleIndices[5] = 3;
            triangleIndices[6] = 0;
            triangleIndices[7] = 3;
            triangleIndices[8] = 4;

            this.shapes.DrawPolygonFill(vertices, triangleIndices, transform, Color.White);

            this.shapes.DrawCircleFill(100, 100, 25, 50, Color.White);

            this.shapes.End();

            this.screen.UnSet();
            this.screen.Present(this.sprites);

            base.Draw(gameTime);
        }
    }
}
