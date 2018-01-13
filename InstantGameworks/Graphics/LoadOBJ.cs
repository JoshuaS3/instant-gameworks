using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using OpenTK;

namespace InstantGameworks.Graphics
{
    class LoadOBJ
    {

        public static Tuple< List<Vertex>, List<List<int>> > Import(string objFile)
        {

            string mtlFile = "";

            string[] objData = File.ReadAllLines(@objFile);


            List<Vertex> vertexData = new List<Vertex>();
            List<List<int>> faceData = new List<List<int>>();

            foreach (string line in objData)
            {
                if (line.StartsWith("v "))
                {
                    string newLine = line.Substring(2);
                    string[] data = newLine.Split(new char[] { ' ' });
                    float x = float.Parse(data[0]) * 0.5f + 0.5f;
                    float y = float.Parse(data[1]) * 0.5f + 0.5f;
                    float z = float.Parse(data[2]) * 0.5f + 0.5f;
                    Vertex newVertex = new Vertex(
                        new Vector4(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]), 1.0f),
                        new OpenTK.Graphics.Color4( x, y, z, 255 )
                    );
                    vertexData.Add(newVertex);
                    
                }
                else if (line.StartsWith("f "))
                {
                    string newLine = line.Substring(2);
                    string[] data = newLine.Split(new char[] { ' ' });
                    List<int> thisFaceData = new List<int>();
                    foreach (string a in data)
                    {
                        string thisVertex = a.Split(new char[] { '/', '/' })[0];
                        thisFaceData.Add(int.Parse(thisVertex));
                    }
                    faceData.Add(thisFaceData);
                }
                else if (line.StartsWith("mtllib "))
                {
                    mtlFile = line.Substring(7);
                }
            }

            string[] mtlData = File.ReadAllLines( Directory.GetParent(@objFile).ToString() + "/" + mtlFile );

            return Tuple.Create(vertexData, faceData);


        }

    }
}
