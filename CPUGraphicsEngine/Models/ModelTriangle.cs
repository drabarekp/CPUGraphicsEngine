using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

using CPUGraphicsEngine.ViewEntities;

namespace CPUGraphicsEngine.Models
{
    internal class ModelTriangle
    {
        public ModelPoint p1 { get; private set; }
        public ModelPoint p2 { get; private set; }
        public ModelPoint p3 { get; private set; }

        BaseColor baseColor;

        public ModelTriangle(ModelPoint p1, ModelPoint p2, ModelPoint p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
        public ViewTriangle GenerateViewTriangle()
        {
            return new ViewTriangle(p1.viewPoint, p2.viewPoint, p3.viewPoint, baseColor);
        }

        public void UpdateBaseColor(BaseColor baseColor)
        {
            this.baseColor = baseColor;
        }

        public Vector<float> ModelNormalVector()
        {
            return (p1.modelNormal + p2.modelNormal + p3.modelNormal).SubVector(0, 3) / 3.0f;
        }

        public Vector<float> WorldNormalVector()
        {
            return (p1.worldNormal + p2.worldNormal + p3.worldNormal).SubVector(0, 3) / 3.0f;
        }
    }
}
