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

        public Camera activeCamera;

        private Camera staticBackCamera;
        private Camera staticPinsCamera;
        private Camera followingCamera;
        private Camera movingCamera;

        List<LightSource> lights;
        Reflector reflector;

        float t = 0;

        byte[] backBuffer;
        float[] depthBuffer;

        ShadingMode shadingMode = ShadingMode.FlatShading;

        public Presentation(int sizeX, int sizeY)
        {
            width = sizeX;
            height = sizeY;

            lights = new List<LightSource>();
            lights.Add(new LightSource(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f), 0.80f));
            lights.Add(new LightSource(0, 5, -1));
            lights.Add(new LightSource(10, -10, -10));


            staticBackCamera = new Camera((10, 0, 0), (0, 0, 0), MathF.PI / 4.0f);
            staticPinsCamera = new Camera((5.0f, 5.0f, -10.0f), (0.1f, 6.5f, 0.1f), MathF.PI / 3.0f);
            followingCamera = new Camera((-10, -5, -5), (0, 0, 0), MathF.PI / 4.0f);
            movingCamera = new Camera((-10, -5, -5), (0, 0, 0), MathF.PI / 4.0f);

            activeCamera = staticPinsCamera;

            var M = Matrix<float>.Build;
            var V = Vector<float>.Build;

            backBuffer = new byte[width * height * 4];
            depthBuffer = new float[width * height];

            viewMatrix = activeCamera.CreateViewMatrix();
            projectionMatrix = activeCamera.CreateProjectionMatrix();

            var jsonLoader = new JSONLoader();
            var pinModel1 = jsonLoader.LoadPin(Color.Red, new Vector3(0.0f,7.0f,0.5f), new Vector3(MathF.PI, 0,0), 1.0f);
            var pinModel2 = jsonLoader.LoadPin(Color.Red, new Vector3(0.5f, 6.0f, 0.5f), new Vector3(MathF.PI, 0, 0), 1.0f);
            var pinModel3 = jsonLoader.LoadPin(Color.Red, new Vector3(-0.5f, 6.0f, 0.5f), new Vector3(MathF.PI, 0, 0), 1.0f);
            var ballModel = jsonLoader.LoadBall(Color.Yellow, new Vector3(0, -5.0f, -0.7f), new Vector3(0, 0, 0), 0.7f);
            var floorModel = jsonLoader.LoadFloor(Color.SandyBrown, new Vector3(0, -3.0f, 6.0f), new Vector3(0, -MathF.PI/2.0f, 0), 6.0f);
            var reflectorModel = jsonLoader.LoadReflector(Color.LightGray, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 0.5f);
            
            AddMesh(ballModel);
            AddMesh(pinModel1);
            AddMesh(pinModel2);
            AddMesh(pinModel3);
            AddMesh(reflectorModel);
            AddMesh(floorModel);

            reflector = new Reflector(lights[0], reflectorModel);
            reflector.Move(new Vector3(-3.0f, 0.0f, -2.0f));

            InitializeViewEntities();

            UpdateWorldPositions();
            CalculateScreenPoints();
            UpdateScreenPosition();
            Clear(255,255,255,1);
        }

        public void Iterate(FastBitmap fastBitmap)
        { 
            SetModelsPositionAndIncrementTime();
            UpdateCameras();

            viewMatrix = activeCamera.CreateViewMatrix();
            projectionMatrix = activeCamera.CreateProjectionMatrix();

            UpdateWorldPositions();
            UpdateViewPoints();
            UpdateScreenPosition();
            Render(fastBitmap);
        }

        public void CalculateScreenPoints()
        {
            Matrix<float> transformationMatrix = projectionMatrix * viewMatrix;
            for (int i = 0; i < points.Count; i++)
            {
                viewPoints[i].SetPosition( points[i].CalculateViewPositon(transformationMatrix));
            }
        }
        public void InitializeViewEntities()
        {
            foreach (var point in points)
            {
                viewPoints.Add(point.viewPoint);
            }
            foreach (var tri in triangles)
            {
                viewTriangles.Add(tri.GenerateViewTriangle());
            }
        }
        public void UpdateViewPoints()
        {
            Matrix<float> transformationMatrix = projectionMatrix * viewMatrix;
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
            Clear(255, 255, 255, 1);
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
            if (z < 0.0f) return; // Additional z checking

            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (x + y * width);
            var index4 = index * 4;

            if (depthBuffer[index] <= z)
            {
                return; // Discard
            }

            depthBuffer[index] = z;
            /*
            backBuffer[index4] = (byte)(color.B);
            backBuffer[index4 + 1] = (byte)(color.G);
            backBuffer[index4 + 2] = (byte)(color.R);
            backBuffer[index4 + 3] = (byte)(color.A);*/


            //at the moment pixel putting isnt optimised so i changed order to natural rgb:
            backBuffer[index4] = (byte)(color.R);
            backBuffer[index4 + 1] = (byte)(color.G);
            backBuffer[index4 + 2] = (byte)(color.B);
            backBuffer[index4 + 3] = (byte)(color.A);
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

        public void AddMesh(Pin mesh)
        {
            meshes.Add(mesh);
            points.AddRange(mesh.points);
            triangles.AddRange(mesh.triangles);
        }

        public bool IsBehind(int x, int y, float z)
        {
            if (this.depthBuffer[x + y * width] <= z) return true;
            return false;
        }

        public void ChangeCamera(CameraEnum cameraEnum)
        {
            switch (cameraEnum)
            {
                case CameraEnum.STATIC_BACK:
                    activeCamera = staticBackCamera;
                    break;
                case CameraEnum.STATIC_PINS:
                    activeCamera = staticPinsCamera;
                    break;
                case CameraEnum.FOLLOWING:
                    activeCamera = followingCamera;
                    break;
                case CameraEnum.MOVING:
                    activeCamera = movingCamera;
                    break;
                default:
                    break;
            }
        }

        private void UpdateCameras()
        {
            movingCamera.MoveBehindTarget(meshes[0]);
            followingCamera.FollowTarget(meshes[0]);
        }

        private void SetModelsPositionAndIncrementTime()
        {
            if (t <= 21.0f)
            {
                meshes[0].SetPosition(0, -5.0f + t * 0.8f, -0.7f);
                meshes[0].SetRotationAngle(0.3f * t, 0.0f, 0.0f);
            }

            reflector.Rotate(new Vector3(0.0f, 0.0f, 0.1f));
            if(t % 16.0f <= 8.0f)
                reflector.Move(new Vector3(0.1f, 0.1f, 0.0f));
            else
                reflector.Move(new Vector3(-0.1f, -0.1f, 0.0f));

            float impactTime = 13.0f;
            if(t >= impactTime && t <= impactTime + MathF.PI/2.0f)
            {
                float tFromImpact = t - impactTime;
                meshes[1].SetPosition(0.5f, 6.0f, 0.5f - tFromImpact * 0.5f);
                meshes[2].SetPosition(0.5f, 6.0f, 0.5f - tFromImpact * 0.5f);
                meshes[3].SetPosition(0.5f, 6.0f, 0.5f - tFromImpact * 0.5f);
                meshes[1].SetRotationAngle(MathF.PI + tFromImpact, 0.0f, 0.0f);
                meshes[2].SetRotationAngle(MathF.PI + tFromImpact, -tFromImpact, 0.0f);
                meshes[3].SetRotationAngle(MathF.PI + tFromImpact, tFromImpact, 0.0f);

            }

            t += 0.1f;
        }
    }
}
