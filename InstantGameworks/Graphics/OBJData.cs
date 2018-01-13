using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace InstantGameworks.Graphics
{
    class OBJData
    {

        public static Tuple< List<List<float>>, List<List<int>> > Import(string objFile)
        {

            string mtlFile = "";

            string[] objData = File.ReadAllLines(@objFile);


            List<List<float>> vertexData = new List<List<float>>();
            List<List<int>> faceData = new List<List<int>>();
            

            foreach (string line in objData)
            {
                if (line.StartsWith("v "))
                {
                    string newLine = line.Substring(2);
                    string[] data = newLine.Split(new char[] { ' ' });
                    List<float> thisVertexData = new List<float>();
                    foreach (string a in data)
                    {
                        thisVertexData.Add(float.Parse(a));
                    }
                    vertexData.Add(thisVertexData);
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
