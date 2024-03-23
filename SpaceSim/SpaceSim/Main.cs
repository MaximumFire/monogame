using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using Flat.Graphics;
using Flat.Input;
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

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
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

        protected override void Update(GameTime gameTime)
        {
            FlatKeyboard keyboard = FlatKeyboard.Instance;
            keyboard.Update();

            FlatMouse mouse = FlatMouse.Instance;
            mouse.Update();

            // Handling Input Goes Here

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

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            this.shapes.Begin(this.camera);

            // Drawing Shapes Goes Here
            this.shapes.DrawCircleFill(100, 100, 25, 50, Color.White);

            this.shapes.End();
            this.screen.UnSet();
            this.screen.Present(this.sprites);

            base.Draw(gameTime);
        }
    }
}
