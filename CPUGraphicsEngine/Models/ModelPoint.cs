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
        public Vector<float> modelNormal { get; private set; }
        public Vector<float> worldNormal { get; private set; }
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
            worldNormal = V.Dense(new float[] {normalX , normalY, normalZ });
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
            worldNormal = Vector<float>.Build.DenseOfEnumerable((modelMatrix * modelNormal).Take(3));
        }
        public Vector<float> CalculateViewPositon(Matrix<float> transformationMatrix)
        {
            return transformationMatrix * worldPosition;
        }

        public void SetNormalVector(float nx, float ny, float nz)
        {
            var V = Vector<float>.Build;
            modelNormal = V.Dense(new float[] { nx, ny, nz, 0.0f });
            modelNormal = modelNormal.Normalize(2);
        }
    }
}
