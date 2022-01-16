using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace CPUGraphicsEngine.ViewEntities
{
    internal class Camera
    {
        float fov;
        Vector<float> position;
        Vector<float> target;

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
                {xAxis[0], yAxis[0], zAxis[0], position[0] },
                {xAxis[1], yAxis[1], zAxis[1], position[1]},
                {xAxis[2], yAxis[2], zAxis[2], position[2]},
                {0.0f, 0.0f, 0.0f, 1.0f }
            };
            return Matrix<float>.Build.DenseOfArray(viewArray);
        }
        private Vector<float> Cross(Vector<float> v1, Vector<float> v2)
        {
            return Vector<float>.Build.Dense(new float[] 
            { 
                v1[1]*v2[2] - v1[2]*v2[1],
                v1[2]*v2[0] - v1[0]*v2[2],
                v1[0]*v2[1] - v1[1]*v2[0]});
        }

    }
}
