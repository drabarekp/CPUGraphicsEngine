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
        public Vector<float> position;
        public ViewPoint viewPoint;

        public ModelPoint(float x, float y, float z)
        {
            viewPoint = new ViewPoint();
            var V = Vector<float>.Build;
            position = V.Dense(new float[] { x, y, z, 1.0f });
        }

        public ViewPoint CalculateViewPoint(Matrix<float> transformationMatrix)
        {
            return new ViewPoint(transformationMatrix * position);
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
    }
}
