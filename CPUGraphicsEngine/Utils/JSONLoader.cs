using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using CPUGraphicsEngine.Models;
using CPUGraphicsEngine.Utils;

namespace CPUGraphicsEngine
{
    internal class JSONLoader
    {
        string folderPath = "..\\..\\..\\..\\ModelsArchive\\";
        string pinFilename = "veryLowPolyPin.babylon";
        string ballFilename = "ball30.babylon";
        string reflectorFilename = "reflector.babylon";
        string floorFilename = "plane30cube.babylon";

        private Pin LoadJSONFile(string filename)
        {
            var meshes = new List<ModelTriangle>();
            var stream = File.OpenRead(filename);
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
                //item.Value.worldNormal = item.Value.worldNormal.Normalize(2);
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

        public Pin LoadPin(System.Drawing.Color color, Vector3 startPosition, Vector3 startRotation, float scale)
        {
            var pin =  LoadJSONFile(folderPath + pinFilename);
            InitializeMesh(pin, color, startPosition, startRotation, scale);

            return pin;
        }

        public Pin LoadBall(System.Drawing.Color color, Vector3 startPosition, Vector3 startRotation, float scale)
        {
            var ball = LoadJSONFile(folderPath + ballFilename);
            InitializeMesh(ball, color, startPosition, startRotation, scale);

            return ball;
        }
        public Pin LoadFloor(System.Drawing.Color color, Vector3 startPosition, Vector3 startRotation, float scale)
        {
            Pin floor = LoadJSONFile(folderPath + floorFilename);
            /*foreach(var modelPoint in floor.points)
            {
                modelPoint.modelPosition[0] *=0.1f;
            }*/
            InitializeMesh(floor, color, startPosition, startRotation, scale);

            return floor;
        }

        public Pin LoadReflector(System.Drawing.Color color, Vector3 startPosition, Vector3 startRotation, float scale)
        {
            Pin reflector = LoadJSONFile(folderPath + reflectorFilename);
            InitializeMesh(reflector, color, startPosition, startRotation, scale);

            return reflector;
        }

        private void InitializeMesh(Pin mesh, System.Drawing.Color color, Vector3 startPosition, Vector3 startRotation, float scale)
        {
            mesh.SetColor(color);
            mesh.Move(startPosition.x, startPosition.y, startPosition.z);
            mesh.Rotate(startRotation.x, startRotation.y, startRotation.z);
            mesh.scaleFactor = scale;
        }

    }
}
