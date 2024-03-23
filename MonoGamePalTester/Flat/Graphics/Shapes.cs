using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flat.Graphics
{
    public sealed class Shapes : IDisposable
    {
        public static readonly float MinLineThickness = 1f;
        public static readonly float MaxLineThickness = 10f;

        bool isDisposed;

        private Game game;
        private BasicEffect effect;

        private VertexPositionColor[] verticies;
        private int[] indices;

        private int shapeCount;
        private int vertexCount;
        private int indexCount;

        private bool isStarted;

        private Camera camera;

        public Shapes(Game game)
        {
            this.isDisposed = false;
            this.game = game ?? throw new ArgumentNullException("game");

            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.TextureEnabled = false;
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.World = Matrix.Identity;
            this.effect.View = Matrix.Identity;
            this.effect.Projection = Matrix.Identity;
            this.effect.VertexColorEnabled = true;

            const int MaxVertexCount = 1024;
            const int MaxIndexCount = MaxVertexCount * 3;

            this.verticies = new VertexPositionColor[MaxVertexCount];
            this.indices = new int[MaxIndexCount];

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;

            this.isStarted = false;

            this.camera = null;
        }

        public void Dispose()
        {
            if (isDisposed) return;

            this.effect?.Dispose();
            this.isDisposed = true;
        }

        public void Begin(Camera camera)
        {
            if (this.isStarted) throw new Exception("batching is already started.");

            if (camera is null)
            {
                Viewport vp = this.game.GraphicsDevice.Viewport;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);
                this.effect.View = Matrix.Identity;
            }
            else
            {
                camera.UpdateMatricies();

                this.effect.View = camera.View;
                this.effect.Projection = camera.Projection;
            }

            this.camera = camera;

            this.isStarted = true;
        }

        public void End()
        {   
            this.Flush();
            this.isStarted = false;
        }

        public void Flush()
        {
            if (this.shapeCount == 0)
            {
                return;
            }
            
            this.EnsureStarted();

            foreach(EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.verticies,
                    0,
                    this.vertexCount,
                    this.indices,
                    0,
                    this.indexCount / 3);
            }

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;
        }

        private void EnsureStarted()
        {
            if (!this.isStarted) throw new Exception("batching was never started.");
        }

        private void EnsureSpace(int shapeVertexCount, int shapeIndexCount)
        {
            if (shapeVertexCount > this.verticies.Length)
            {
                throw new Exception("Maximum shape vertex count is: " + this.verticies.Length);
            }

            if (shapeIndexCount > this.indices.Length)
            {
                throw new Exception("Maximum shape index count is: " + this.indices.Length);
            }

            if (this.vertexCount + shapeVertexCount > this.verticies.Length
                || this.indexCount + shapeIndexCount > this.indices.Length)
            {
                this.Flush();
            }
        }

        public void DrawRectangleFill(float x, float y, float width, float height, Color color)
        {
            this.EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;
            
            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = x;
            float right = x + width;
            float bottom = y;
            float top = y + height;

            Vector2 a = new Vector2(left, top);
            Vector2 b = new Vector2(right, top);
            Vector2 c = new Vector2(right, bottom);
            Vector2 d = new Vector2(left, bottom);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

            this.shapeCount++;
        }

        public void DrawLine(Vector2 a, Vector2 b, float thickness, Color color)
        {
            this.DrawLine(a.X, a.Y, b.X, b.Y, thickness, color);
        }

        public void DrawLine(float ax, float ay, float bx, float by, float thickness, Color color)
        {
            this.EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            thickness = Util.Clamp(thickness, Shapes.MinLineThickness, Shapes.MaxLineThickness);
            thickness += 1f;

            if (this.camera != null)
            {
                thickness *= this.camera.Z / this.camera.BaseZ;
            }

            float halfThickness = thickness / 2;

            float e1x = bx - ax;
            float e1y = by - ay;

            Util.Normalise(ref e1x, ref e1y);

            e1x *= halfThickness;
            e1y *= halfThickness;

            float e2x = -e1x;
            float e2y = -e1y;

            float n1x = -e1y;
            float n1y = e1x;

            float n2x = -n1x;
            float n2y = -n1y;

            float q1x = ax + n1x + e2x;
            float q1y = ay + n1y + e2y;
            
            float q2x = bx + n1x + e1x;
            float q2y = by + n1y + e1y;

            float q3x = bx + n2x + e1x;
            float q3y = by + n2y + e1y;

            float q4x = ax + n2x + e2x;
            float q4y = ay + n2y + e2y;

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(q1x, q1y, 0f), color);
            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(q2x, q2y, 0f), color);
            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(q3x, q3y, 0f), color);
            this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(q4x, q4y, 0f), color);

            this.shapeCount++;
        }

        public void DrawRectangle(float x, float y, float width, float height, float thickness, Color color)
        {
            float left = x;
            float right = x + width;
            float bottom = y;
            float top = y + height;

            this.DrawLine(left, top, right, top, thickness, color);
            this.DrawLine(right, top, right, bottom, thickness, color);
            this.DrawLine(right, bottom, left, bottom, thickness, color);
            this.DrawLine(left, bottom, left, top, thickness, color);
        }

        public void DrawCircle(float x, float y, float radius, int points, float thickness, Color color)
        {
            const int minPoints = 3;
            const int maxPoints = 256;

            points = Util.Clamp(points, minPoints, maxPoints);

            float rotation = MathHelper.TwoPi / (float)points;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;

            float bx = 0f;
            float by = 0f;

            for (int i = 0; i < points; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                this.DrawLine(ax + x, ay + y, bx + x, by + y, thickness, color);

                ax = bx;
                ay = by;
            }
        }

        public void DrawCircleFill(float x, float y, float radius, int points, Color color)
        {
            this.EnsureStarted();
            const int minPoints = 3;
            const int maxPoints = 256;

            int shapeVertexCount = Util.Clamp(points, minPoints, maxPoints);
            int shapeTriangleCount = shapeVertexCount - 2;
            int shapeIndexCount = shapeVertexCount * 3;

            EnsureSpace(shapeVertexCount, shapeIndexCount);

            for (int i = 0; i < shapeTriangleCount; i++)
            {
                this.indices[this.indexCount++] = 0 + this.vertexCount;
                this.indices[this.indexCount++] = i + 1 + this.vertexCount;
                this.indices[this.indexCount++] = i + 2 + this.vertexCount;
            }


            float rotation = MathHelper.TwoPi / (float)points;

            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;

            for (int i = 0; i < shapeVertexCount; i++)
            {
                float x1 = ax;
                float y1 = ay;

                this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(x1 + x, y1 + y, 0), color);

                ax = cos * x1 - sin * y1;
                ay = sin * x1 + cos * y1;
            }

            this.shapeCount++;
        }

        public void DrawPolygon(Vector2[] vertices, Matrix transform, float thickness, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % vertices.Length];

                a = Vector2.Transform(a, transform);
                b = Vector2.Transform(b, transform);

                this.DrawLine(a, b, thickness, color);
            }
        }

        public void DrawPolygonFill(Vector2[] vertices, int[] triangleIndices, Matrix transform, Color color)
        {
#if DEBUG
            if (vertices is null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            if (triangleIndices is null)
            {
                throw new ArgumentNullException(nameof(triangleIndices));
            }

            if (vertices.Length < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(vertices));
            }

            if (triangleIndices.Length < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(triangleIndices));
            }
#endif

            this.EnsureSpace(vertices.Length, triangleIndices.Length);

            for (int i = 0; i < triangleIndices.Length; i++)
            {
                this.indices[this.indexCount++] = triangleIndices[i] + this.vertexCount;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 vertex = vertices[i];
                vertex = Vector2.Transform(vertex, transform);

                this.verticies[this.vertexCount++] = new VertexPositionColor(new Vector3(vertex.X, vertex.Y, 0f), color);
            }

            this.shapeCount++;
        }
    }
}
