using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPUGraphicsEngine.Models;
using CPUGraphicsEngine.Utils;

namespace CPUGraphicsEngine.ViewEntities
{
    internal class Reflector
    {
        LightSource light;
        Pin mesh;

        public Reflector(LightSource light, Pin mesh)
        {
            this.light = light;
            this.mesh = mesh;
        }
        public void Move(Vector3 move)
        {
            light.Move(move.x, move.y, move.z);
            mesh.Move(move.x, move.y, move.z);
        }

        public void Rotate(Vector3 rotation)
        {
            light.Rotate(rotation.x, rotation.y, rotation.z);
            mesh.Rotate(rotation.x, rotation.y, rotation.z);
        }
    }
}
