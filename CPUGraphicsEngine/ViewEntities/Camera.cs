using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

using CPUGraphicsEngine.Models;
using CPUGraphicsEngine.Utils;

namespace CPUGraphicsEngine.ViewEntities
{
    internal class Camera
    {
        float e;
        float n = 1;
        float f = 10;
        float a = 1;

        public Vector<float> position;
        Vector<float> target;
        readonly Vector<float> distanceFromTarget;

        public Camera((float X, float Y, float Z) position, (float X, float Y, float Z) target, float fov)
        {
            var V = Vector<float>.Build;
            this.position = V.Dense(new float[] { position.X, position.Y, position.Z });
            this.target = V.Dense(new float[] { target.X, target.Y, target.Z });
            distanceFromTarget = Vector<float>.Build.DenseOfArray(new float[] { 0.0000f, -2.0f, -4.0f });
            e = 1 / MathF.Tan(fov);
        }
        public Matrix<float> CreateViewMatrix()
        {
            Vector<float> zAxis = position - target;
            Vector<float> upVector = Vector<float>.Build.Dense(new float[] { 0, 0, 1 });
            zAxis = zAxis.Normalize(2);
            Vector<float> xAxis = Cross(upVector, zAxis);
            xAxis = xAxis.Normalize(2);
            Vector<float> yAxis = Cross(zAxis, xAxis);

            float[,] viewArray =
            {
                {xAxis[0], yAxis[0], zAxis[0], position[0] }, //nie mogą być same zera w zAxis
                {xAxis[1], yAxis[1], zAxis[1], position[1]},
                {xAxis[2], yAxis[2], zAxis[2], position[2]},
                {0.0f, 0.0f, 0.0f, 1.0f }
            };
            return Matrix<float>.Build.DenseOfArray(viewArray).Inverse();
        }

        public Matrix<float> CreateProjectionMatrix()
        {
            float[,] projectionArray =
                {
                {e, 0,0,0 },
                {0, e/a, 0, 0 },
                {0,0, -(f+n)/(f-n), -(2*f*n)/(f-n) },
                {0,0,-1,0 }
                };

            return Matrix<float>.Build.DenseOfArray(projectionArray);
        }

        private Vector<float> Cross(Vector<float> v1, Vector<float> v2)
        {
            return Vector<float>.Build.Dense(new float[] 
            { 
                v1[1]*v2[2] - v1[2]*v2[1],
                v1[2]*v2[0] - v1[0]*v2[2],
                v1[0]*v2[1] - v1[1]*v2[0]});
        }

        public void Move(float x, float y, float z)
        {
            position[0] += x;
            position[1] += y;
            position[2] += z;
        }

        public void SetTarget(float x, float y, float z)
        {
            target[0] = x;
            target[1] = y;
            target[2] = z;
        }

        public void FollowTarget(Pin mesh)
        {
            target = mesh.Position;
        }

        public void MoveBehindTarget(Pin target)
        {
            this.target = target.Position;
            this.position = target.Position + distanceFromTarget;
        }

    }
}
