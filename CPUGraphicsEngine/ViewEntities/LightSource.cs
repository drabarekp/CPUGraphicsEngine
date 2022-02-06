using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

using CPUGraphicsEngine.Utils;

namespace CPUGraphicsEngine.ViewEntities
{
    internal class LightSource
    {
        public Vector<float> position;
        public bool reflector;
        public float beamAngleCosine;
        public Vector<float> direction;

        public LightSource(float x, float y, float z)
        {
            position = Vector<float>.Build.Dense(new float[] { x, y, z });
            reflector = false;
        }
        public LightSource(Vector3 position, Vector3 direction, float beamAngleCosine)
        {
            this.position = Vector<float>.Build.Dense(new float[] { position.x, position.y, position.z });
            this.direction = Vector<float>.Build.Dense(new float[] { direction.x, direction.y, direction.z });
            this.direction = this.direction.Normalize(2);
            this.beamAngleCosine = beamAngleCosine;
            reflector = true;
        }
        public void Move(float x, float y, float z)
        {
            position[0] += x;
            position[1] += y;
            position[2] += z;
        }

        public bool IsBeingLit(Vector<float> pointPosition)
        {
            if (!reflector) return true;

            Vector<float> directionToPoint = pointPosition - position;
            directionToPoint = directionToPoint.Normalize(2);

            if (directionToPoint.DotProduct(direction) > beamAngleCosine) return true;
            return false;
        }
    }
}
