using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using CPUGraphicsEngine.Models;

namespace CPUGraphicsEngine
{
    public struct ScanLineData
    {
        public int currentY;
        public float ndotla;
        public float ndotlb;
        public float ndotlc;
        public float ndotld;
    }

    internal class JSONLoader
    {
        public Pin LoadJSONFile()
        {
            var meshes = new List<ModelTriangle>();
            string filename;
            var stream = File.OpenRead("pin-030.babylon");
            string jsonString = "";
            StreamReader reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                jsonString += reader.ReadLine();
            }

            JsonDocument document = JsonDocument.Parse(jsonString);
            JsonElement root = document.RootElement;
            JsonElement meshesDto = root.GetProperty("meshes");
            JsonElement positionsEl = new JsonElement();
            JsonElement indicesEl = new JsonElement();
            JsonElement normalsEl = new JsonElement();

            foreach (var x in meshesDto.EnumerateArray())
            { 
                positionsEl = x.GetProperty("positions");
                indicesEl = x.GetProperty("indices");
                normalsEl = x.GetProperty("normals");
            }

            var nums = new List<float>();
            var indices = new List<int>();
            var normals = new List<float>();

            foreach (var p in positionsEl.EnumerateArray())
            {
                nums.Add(p.GetSingle());
            }
            foreach (var p in indicesEl.EnumerateArray())
            {
                indices.Add(p.GetInt32());
            }
            foreach (var p in normalsEl.EnumerateArray())
            {
                normals.Add(p.GetSingle());
            }

            var verticesCount = nums.Count / 3;
            var facesCount = indices.Count / 3;

            Pin pin = new Pin();
            for (var index = 0; index < verticesCount; index++)
            {
                var x = (float)nums[index * 3];
                var y = (float)nums[index * 3 + 1];
                var z = (float)nums[index * 3 + 2];

                var nx = (float)normals[index * 3];
                var ny = (float)normals[index * 3 + 1];
                var nz = (float)normals[index * 3 + 2];

                pin.points.Add(new ModelPoint(x, y, z, nx, ny, nz));
            }

            for (var index = 0; index < facesCount; index++)
            {
                var a = (int)indices[index * 3];
                var b = (int)indices[index * 3 + 1];
                var c = (int)indices[index * 3 + 2];
                pin.triangles.Add(new ModelTriangle(pin.points[a], pin.points[b], pin.points[c])); //idk if in triangle should me indexes or point references
            }

            //pin.Move(0.0f, 1, 0);
            return pin;
            
        }
    }
}
