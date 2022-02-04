using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

using CPUGraphicsEngine.ViewEntities;

namespace CPUGraphicsEngine.Models
{
    internal class ModelPoint
    {

        // modelPosition
        public Vector<float> position;
        public Vector<float> normal;
        public ViewPoint viewPoint;

        public ModelPoint(float x, float y, float z)
        {
            viewPoint = new ViewPoint(this);
            var V = Vector<float>.Build;
            position = V.Dense(new float[] { x, y, z, 1.0f });
        }
        public ModelPoint(float x, float y, float z, float normalX, float normalY, float normalZ)
        {
            viewPoint = new ViewPoint(this);
            var V = Vector<float>.Build;
            position = V.Dense(new float[] { x, y, z, 1.0f });
            normal = V.Dense(new float[] {normalX , normalY, normalZ });
        }

        public ViewPoint CalculateViewPoint(Matrix<float> transformationMatrix)
        {
            return new ViewPoint(transformationMatrix * position, this);
        }
        public void UpdateViewPoint(Matrix<float> transformationMatrix)
        {
            viewPoint.SetPosition(transformationMatrix * position);
        }
        public Vector<float> CalculateViewPositon(Matrix<float> transformationMatrix)
        {
            return transformationMatrix * position;
        }

        public void Move(float x, float y, float z)
        {
            position[0] += x;
            position[1] += y;
            position[2] += z;
        }
        public void SetNormalVector(float nx, float ny, float nz)
        {
            var V = Vector<float>.Build;
            normal = V.Dense(new float[] { nx, ny, nz });
        }
    }
}
