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
        public Vector<float> modelPosition;
        public Vector<float> worldPosition;
        public Vector<float> normal;
        public ViewPoint viewPoint;

        public ModelPoint(float x, float y, float z)
        {
            viewPoint = new ViewPoint(this);
            var V = Vector<float>.Build;
            modelPosition = V.Dense(new float[] { x, y, z, 1.0f });
        }
        public ModelPoint(float x, float y, float z, float normalX, float normalY, float normalZ)
        {
            viewPoint = new ViewPoint(this);
            var V = Vector<float>.Build;
            modelPosition = V.Dense(new float[] { x, y, z, 1.0f });
            normal = V.Dense(new float[] {normalX , normalY, normalZ });
        }

        public ViewPoint CalculateViewPoint(Matrix<float> transformationMatrix)
        {
            return new ViewPoint(transformationMatrix * worldPosition, this);
        }
        public void UpdateViewPoint(Matrix<float> transformationMatrix)
        {
            viewPoint.SetPosition(transformationMatrix * worldPosition);
        }
        public void UpdateWorldPosition(Matrix<float> modelMatrix)
        {
            worldPosition = modelMatrix * modelPosition;
        }
        public Vector<float> CalculateViewPositon(Matrix<float> transformationMatrix)
        {
            return transformationMatrix * worldPosition;
        }

        public void Move(float x, float y, float z)
        {
            worldPosition[0] += x;
            worldPosition[1] += y;
            worldPosition[2] += z;
        }
        public void SetNormalVector(float nx, float ny, float nz)
        {
            var V = Vector<float>.Build;
            normal = V.Dense(new float[] { nx, ny, nz });
        }
    }
}
