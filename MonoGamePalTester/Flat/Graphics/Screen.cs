using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flat.Graphics
{
    public sealed class Screen : IDisposable
    {
        private readonly static int MinDim = 64;
        private readonly static int MaxDim = 4096;

        private bool isDisposed;
        private Game game;
        private RenderTarget2D target;
        private bool isSet;

        public int Width
        {
            get { return this.target.Width; }
        }

        public int Height
        {
            get { return this.target.Height; }
        }

        public Screen(Game game, int width, int height)
        {
            width = Util.Clamp(width, MinDim, MaxDim);
            height = Util.Clamp(height, MinDim, MaxDim);

            this.game = game ?? throw new ArgumentNullException("game");

            this.target = new RenderTarget2D(this.game.GraphicsDevice, width, height);
            isSet = false;
        }

        public void Dispose()
        {
            if (isDisposed) return;

            this.target?.Dispose();
            isDisposed = true;
        }

        public void Set()
        {
            if (isSet) throw new Exception("Render target is already set.");

            this.game.GraphicsDevice.SetRenderTarget(this.target);
            isSet = true;
        }

        public void UnSet()
        {
            if (!isSet) throw new Exception("Render target is not set.");

            this.game.GraphicsDevice.SetRenderTarget(null);
            isSet = false;
        }

        public void Present(Sprites sprites, bool textureFiltering = true)
        {
            if (sprites is null) throw new ArgumentNullException("sprites");

#if DEBUG
            this.game.GraphicsDevice.Clear(Color.HotPink);
#else            
            this.game.GraphicsDevice.Clear(Color.Black);
#endif
            Rectangle destinationRectangle = this.CalculateDestinationRectangle();

            sprites.Begin(null, textureFiltering);
            sprites.Draw(this.target, null, destinationRectangle, Color.White);
            sprites.End();
        }

        internal Rectangle CalculateDestinationRectangle()
        {
            Rectangle backbufferBounds = this.game.GraphicsDevice.PresentationParameters.Bounds;

            float backbufferAspectRatio = (float)backbufferBounds.Width / (float)backbufferBounds.Height;
            float screenAspectRato = (float)this.Width / (float)this.Height;

            float rx = 0f;
            float ry = 0f;
            float rw = backbufferBounds.Width;
            float rh = backbufferBounds.Height;

            if (backbufferAspectRatio > screenAspectRato)
            {
                rw = rh * screenAspectRato;
                rx = ((float)backbufferBounds.Width - rw) / 2f;
            }
            else if (backbufferAspectRatio < screenAspectRato)
            {
                rh = rw / screenAspectRato;
                ry = ((float)backbufferBounds.Height - rh) / 2f;
            }

            Rectangle result = new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
            return result;
        }
    }
}
