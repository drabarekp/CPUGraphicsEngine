using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUGraphicsEngine
{
    internal class MeshDTO
    {
        string name;
        string id;
        string materialId;
        int billboardMode;
        float[] position;
        float[] rotation;
        float[] scaling;
        bool isVisible;
        bool isEnabled;
        bool pickable;

        float[] positions;
        float[] normals;
    }
}
