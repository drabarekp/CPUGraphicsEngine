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

        public void Rotate(float xAngle, float yAngle, float zAngle)
        {

            Vector<float> direction4 = Vector<float>.Build.Dense(new float[] { direction[0], direction[1], direction[2], 1.0f });
            var M = Matrix<float>.Build;
            float[,] rotationX = new float[4, 4]
            {
                {1.0f, 0.0f, 0.0f, 0.0f },
                {0.0f, MathF.Cos(xAngle), -MathF.Sin(xAngle), 0.0f },
                {0.0f, MathF.Sin(xAngle), MathF.Cos(xAngle), 0.0f },
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            float[,] rotationY = new float[4, 4]
            {
                {MathF.Cos(yAngle), 0.0f, MathF.Sin(yAngle), 0.0f },
                {0.0f, 1.0f, 0.0f, 0.0f },
                {-MathF.Sin(yAngle), 0.0f, MathF.Cos(yAngle), 0.0f },
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            float[,] rotationZ = new float[4, 4]
            {
                {MathF.Cos(zAngle), -MathF.Sin(zAngle), 0.0f, 0.0f },
                {MathF.Sin(zAngle), MathF.Cos(zAngle), 0.0f, 0.0f },
                {0.0f, 0.0f, 1.0f, 0.0f },
                {0.0f, 0.0f, 0.0f, 1.0f }
            };

            var xRotationMatrix = M.DenseOfArray(rotationX);
            var yRotationMatrix = M.DenseOfArray(rotationY);
            var zRotationMatrix = M.DenseOfArray(rotationZ);

            direction = (xRotationMatrix * yRotationMatrix * zRotationMatrix * direction4).SubVector(0, 3);
        }
    }
}
