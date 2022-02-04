using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace CPUGraphicsEngine.Models
{
    internal class Pin
    {
        public List<ModelPoint> points = new List<ModelPoint>();
        public List<ModelTriangle> triangles = new List<ModelTriangle>();

        Vector<float> position;
        float xAngle = 0.0f;
        float yAngle = 0.0f;
        float zAngle = 0.0f;
        float scaleFactor = 1.0f;

        public void Render()
        {

        }

        public void Move(float x, float y, float z)
        {
            foreach(var point in points)
            {
                point.Move(x, y, z);
            }
        }

        Matrix<float> GetModelMatrix()
        {
            var M = Matrix<float>.Build;

            float[,] translation = new float[4, 4]
            {
                {1.0f, 0.0f, 0.0f, position[0]},
                {0.0f, 1.0f, 0.0f, position[1] },
                {0.0f, 0.0f, 1.0f, position[2]},
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            float[,] scalling = new float[4, 4]
            {
                {scaleFactor, 0.0f,0.0f,0.0f },
                {0.0f, scaleFactor, 0.0f, 0.0f },
                {0.0f, 0.0f, scaleFactor, 0.0f },
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            float[,] rotationX = new float[4, 4]
            {
                {1.0f, 0.0f, 0.0f, 0.0f },
                {0.0f, MathF.Cos(xAngle), -MathF.Sin(xAngle), 0.0f },
                {0.0f, MathF.Sin(xAngle), MathF.Cos(xAngle), 0.0f },
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            float[,] rotationY = new float[4, 4]
            {
                {MathF.Cos(yAngle), 0.0f, MathF.Sin(yAngle), 0.0f },
                {0.0f, 1.0f, 0.0f, 0.0f },
                {-MathF.Sin(yAngle), 0.0f, MathF.Cos(yAngle), 0.0f },
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            float[,] rotationZ = new float[4, 4]
            {
                {MathF.Cos(zAngle), -MathF.Sin(zAngle), 0.0f, 0.0f },
                {MathF.Sin(zAngle), MathF.Cos(zAngle), 0.0f, 0.0f },
                {0.0f, 0.0f, 1.0f, 0.0f },
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            var xRotationMatrix = M.DenseOfArray(rotationX);
            var yRotationMatrix = M.DenseOfArray(rotationY);
            var zRotationMatrix = M.DenseOfArray(rotationZ);
            var translationMatrix = M.DenseOfArray(translation);
            var scallingMatrix = M.DenseOfArray(scalling);

            return translationMatrix * xRotationMatrix * yRotationMatrix* zRotationMatrix * scallingMatrix;

        }

    }
}
