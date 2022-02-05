using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Hazdryx.Drawing;
using System.Drawing;

using CPUGraphicsEngine.Models;
using CPUGraphicsEngine.ViewEntities;
using CPUGraphicsEngine.Utils;

namespace CPUGraphicsEngine
{
    internal class Presentation
    {
        public int width = 800;
        public int height = 800;

        Matrix<float> projectionMatrix;
        public Matrix<float> viewMatrix;
        public Matrix<float> modelMatrix;

        public List<ModelPoint> points = new List<ModelPoint>();
        public List<ModelTriangle> triangles = new List<ModelTriangle>();
        public List<Pin> meshes = new List<Pin>();

        public List<ViewPoint> viewPoints = new List<ViewPoint>();
        public List<ViewTriangle> viewTriangles = new List<ViewTriangle>();

        public Camera camera;
        List<LightSource> lights;

        float e = 1 / MathF.Tan(MathF.PI / 3.0f);
        float n = 1;
        float f = 100;
        float a = 1;

        float alpha = 0;

        byte[] backBuffer;
        float[] depthBuffer;

        ShadingMode shadingMode = ShadingMode.FlatShading;

        public Presentation()
        {
            lights = new List<LightSource>();
            lights.Add(new LightSource(10, 0, 0));
            camera = new Camera((0, 4, 2), (0, 0, 1));

            var M = Matrix<float>.Build;
            var V = Vector<float>.Build;

            backBuffer = new byte[width * height * 4];
            depthBuffer = new float[width * height];

            float[,] projectionArray =
                {
                {e, 0,0,0 },
                {0, e/a, 0, 0 },
                {0,0, -(f+n)/(f-n), -(2*f*n)/(f-n) },
                {0,0,-1,0 }
                };
            projectionMatrix = M.DenseOfArray(projectionArray);

            float[,] modelArray =
            {
                {MathF.Cos(alpha),-MathF.Sin(alpha), 0,0.1f },
                {MathF.Sin(alpha), MathF.Cos(alpha), 0,0.2f },
                {0,0,1,0.3f },
                {0,0,0,1 }
            };
            modelMatrix = M.DenseOfArray(modelArray);

            /*float[,] viewArray =
            {
                {-1,0, 0,0 },
                {0,0,1,0 },
                {0,1,0,-5},
                {0,0,0,1 }
            };
            viewMatrix = M.DenseOfArray(viewArray);*/
            viewMatrix = camera.CreateViewMatrix();

            var jsonLoader = new JSONLoader();
            var pin = jsonLoader.LoadJSONFile();

            meshes.Add(pin);
            points = pin.points;
            triangles = pin.triangles;

            foreach(var point in points)
            {
                viewPoints.Add(point.viewPoint);
            }
            foreach(var tri in triangles)
            {
                viewTriangles.Add(tri.GenerateViewTriangle());
            }


            UpdateWorldPositions();
            CalculateScreenPoints();
            UpdateScreenPosition();
            Clear(0,0,0,1);
        }

        public void CalculateScreenPoints()
        {
            Matrix<float> transformationMatrix = projectionMatrix * viewMatrix;
            for (int i = 0; i < points.Count; i++)
            {
                viewPoints[i].SetPosition( points[i].CalculateViewPositon(transformationMatrix));
            }
        }
        public void UpdateViewPoints()
        {
            Matrix<float> transformationMatrix = projectionMatrix * viewMatrix * modelMatrix;
            foreach (var p in points)
            {
                p.UpdateViewPoint(transformationMatrix);
            }
        }
        public void UpdateScreenPosition()
        {
            foreach (var v in viewPoints)
                v.UpdateScreenPosition(width, height);
        }

        public void Render(FastBitmap fastBitmap)
        {

            switch (shadingMode) {
                case ShadingMode.FlatShading:
                    RenderTringlesFlat();
                    break;
                case ShadingMode.GouraudShading:
                    RenderTringlesGouraud();
                    break;
                case ShadingMode.PhongShading:
                    RenderTringlesPhong();
                    break;
                default:
                    break;
            }

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Color c;
                    c = Color.FromArgb(backBuffer[4 * (x + y * width)], backBuffer[4 * (x + y * width) + 1], backBuffer[4 * (x + y * width) + 2]);
                    fastBitmap.Set(x, y, c); //todo optimisation of color
                }
            Clear(0, 0, 0, 1);
        }

        private void RenderTringlesFlat()
        {
            foreach (var t in viewTriangles)
            {
                t.DrawTriangleFlat(t.baseColor, this, lights);
            }
        }
        private void RenderTringlesGouraud()
        {
            foreach (var t in viewTriangles)
            {
                t.DrawTriangleGouraud(t.baseColor, this, lights);
            }
        }
        private void RenderTringlesPhong()
        {
            foreach (var t in viewTriangles)
            {
                t.DrawTrianglePhong(t.baseColor, this, lights);
            }
        }
        public void Render(Graphics g)
        {
            /*foreach (var p in viewPoints)
            {
                p.Draw(g, width, height);
            }*/
        }
        public void Clear(byte r, byte g, byte b, byte a)
        {
            // Clearing Back Buffer
            for (var index = 0; index < backBuffer.Length; index += 4)
            {
                // BGRA is used by Windows instead by RGBA in HTML5
                backBuffer[index] = b;
                backBuffer[index + 1] = g;
                backBuffer[index + 2] = r;
                backBuffer[index + 3] = a;
            }

            // Clearing Depth Buffer
            for (var index = 0; index < depthBuffer.Length; index++)
            {
                depthBuffer[index] = float.MaxValue;
            }
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, float z, BaseColor color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (x + y * width);
            var index4 = index * 4;

            if (depthBuffer[index] < z)
            {
                return; // Discard
            }

            depthBuffer[index] = z;

            backBuffer[index4] = (byte)(color.B);
            backBuffer[index4 + 1] = (byte)(color.G);
            backBuffer[index4 + 2] = (byte)(color.R);
            backBuffer[index4 + 3] = (byte)(color.A);
        }
        public void incAlpha()
        {
            alpha += 0.1f;
            //lights[0].Move(-100, -100, 0);
        }
        public void calcModelMatrix()
        {
            var M = Matrix<float>.Build;
            float[,] modelArray =
            {
                {MathF.Cos(alpha),-MathF.Sin(alpha), 0,0.1f },
                {MathF.Sin(alpha), MathF.Cos(alpha), 0,0.2f },
                {0,0,1,0.3f },
                {0,0,0,1 }
            };
            modelMatrix = M.DenseOfArray(modelArray);
        }

        public void Iterate(FastBitmap fastBitmap)
        {
            Clear(0, 0, 0, 1);
            meshes[0].Move(0.05f, 0.0f, 0.0f);
            meshes[0].Rotate(0.03f, 0.03f, 0.03f);
            //camera.Move(0f, 0, 0.1f);
            viewMatrix = camera.CreateViewMatrix();
            UpdateWorldPositions();
            UpdateViewPoints();
            UpdateScreenPosition();
            Render(fastBitmap);
        }

        public void UpdateWorldPositions()
        {
            foreach(var mesh in meshes)
            {
                mesh.UpdateWorldPositions();
            }
        }

        public void ChangeShading(ShadingMode shading)
        {
            shadingMode = shading;
        }
    }
}
