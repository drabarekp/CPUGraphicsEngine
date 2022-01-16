using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace CPUGraphicsEngine.ViewEntities
{
    internal class LightSource
    {
        public Vector<float> position;


        public LightSource(float x, float y, float z)
        {
            position = Vector<float>.Build.Dense(new float[] { x, y, z });
        }
        public void Move(float x, float y, float z)
        {
            position[0] += x;
            position[1] += y;
            position[2] += z;
        }
    }
}
