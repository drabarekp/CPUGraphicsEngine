using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUGraphicsEngine.Models
{
    internal class Pin
    {
        public List<ModelPoint> points = new List<ModelPoint>();
        public List<ModelTriangle> triangles = new List<ModelTriangle>();

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

    }
}
