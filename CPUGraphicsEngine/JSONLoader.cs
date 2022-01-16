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
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;

        public static Vector3 Add(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public Vector3 Divide(float arg)
        {
            return new Vector3(x / arg, y / arg, z / arg);
        }
    }

    internal class JSONLoader
    {
        public Pin LoadJSONFile()
        {
            var meshes = new List<ModelTriangle>();
            string filename;
            var stream = File.OpenRead("square.babylon");
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
            var pointDictionary = new Dictionary<(float, float, float), ModelPoint>();
            var normalsDictionary = new Dictionary<(float, float, float), HashSet<Vector3>>();
            var indexArray = new ModelPoint[indices.Count];

            for (var index = 0; index < verticesCount; index++)
            {
                var x = (float)nums[index * 3];
                var y = (float)nums[index * 3 + 1];
                var z = (float)nums[index * 3 + 2];

                var nx = (float)normals[index * 3];
                var ny = (float)normals[index * 3 + 1];
                var nz = (float)normals[index * 3 + 2];

                ModelPoint point;
                if (pointDictionary.ContainsKey((x, y, z)))
                {
                    point = pointDictionary[(x, y, z)];
                    normalsDictionary[(x, y, z)].Add(new Vector3(nx, ny, nz));
                }
                else
                {
                    point = new ModelPoint(x, y, z);
                    pointDictionary.Add((x, y, z), point);
                    var list = new HashSet<Vector3>();
                    list.Add(new Vector3(nx, ny, nz));
                    normalsDictionary.Add((x, y, z), list);
                }
                indexArray[index] = point;
            }
                

            foreach(var item in pointDictionary)
            {
                var pos = item.Key;
                var normalsOfThePoint = normalsDictionary[pos];
                Vector3 vect = new Vector3(0, 0, 0);
                foreach (Vector3 v in normalsOfThePoint)
                {
                    vect = Vector3.Add(vect, v);
                }
                vect = vect.Divide(normalsOfThePoint.Count);
                item.Value.SetNormalVector(vect.x, vect.y, vect.z);
                item.Value.normal = item.Value.normal.Normalize(2);
                pin.points.Add(item.Value);
            }

                //TO THIS POINT I HAD REFACTORED

                
                //pin.points.Add(new ModelPoint(x, y, z, nx, ny, nz));
            for (var index = 0; index < facesCount; index++)
            {
                var a = (int)indices[index * 3];
                var b = (int)indices[index * 3 + 1];
                var c = (int)indices[index * 3 + 2];
                pin.triangles.Add(new ModelTriangle(indexArray[a], indexArray[b], indexArray[c])); //idk if in triangle should me indexes or point references
            }

            //pin.Move(0.0f, 1, 0);
            return pin;
            
        }
    }
}
