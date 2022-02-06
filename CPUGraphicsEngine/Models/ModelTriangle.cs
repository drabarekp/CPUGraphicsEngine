using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPUGraphicsEngine.ViewEntities;

namespace CPUGraphicsEngine.Models
{
    internal class ModelTriangle
    {
        ModelPoint p1;
        ModelPoint p2;
        ModelPoint p3;

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
    }
}
