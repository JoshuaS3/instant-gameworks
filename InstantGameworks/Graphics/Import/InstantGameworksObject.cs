using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using OpenTK;
using OpenTK.Graphics;

namespace InstantGameworks.Graphics.Import
{
    public struct Vertex
    {
        public const int Size = (4 + 3 + 4 + 3) * 4;

        public Vector4 Position;
        public Vector3 Normal;
        public Color4 Color;
        public Vector3 TextureCoordinates;
        public Vertex(Vector4 position, Vector3 normal = new Vector3(), Color4 color = new Color4(), Vector3 texCoords = new Vector3())
        {
            Position = position;
            Normal = normal;
            Color = color;
            TextureCoordinates = texCoords;
        }
    }

    class InstantGameworksObject
    {

        public static Tuple<Vertex[], Vector4[], Color4[], Vector3[], Vector3[]> Import(string fileName)
        {
            List<Vector3> positionList = new List<Vector3>();
            List<Vector3> normalList = new List<Vector3>();
            List<Color4> colorList = new List<Color4>();
            List<Vector3> textureList = new List<Vector3>();

            //Internal
            int scaleFactor;

            string fileText;

            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                fileText = reader.ReadString();
            }

            //Parse
            {

                //Scale factor
                {
                    string[] scaleSplit = fileText.Split(new[] { ':' });
                    scaleFactor = int.Parse(scaleSplit[1].Split(new[] { '\n' })[0].TrimStart(' '));
                }
                
                //Vertex data
                {
                    string vertexList = fileText.Split(new[] { 'v' })[1].Split(new[] { '\n' })[1].Split(new[] { '\n' })[0];
                    string[] verticesText = vertexList.Split(new[] { ';' });
                    foreach (string vertex in verticesText)
                    {
                        bool a = false;
                        string[] posData = vertex.Split(new[] { ' ' });
                        List<float> posData2 = new List<float>();
                        foreach (string coord in posData)
                        {
                            if (!string.IsNullOrWhiteSpace(coord))
                            {
                                float pos = float.Parse(coord);
                                posData2.Add(pos / scaleFactor);
                                a = true;
                            }
                        }
                        if (a)
                        {
                            positionList.Add(new Vector3(posData2[0], posData2[1], posData2[2])); // x y z
                        }

                    }
                }


                //Normal data
                {
                    string[] normList = fileText.Split(new[] { 'v' })[1] //after v
                                                 .Split(new[] { 'n' }); //normal
                    if (normList.Length == 2) //check if normals are there
                    {
                        string thisList = normList[1].Split(new[] { '\n' })[1].Split(new[] { '\n' })[0];
                        string[] eachNormal = thisList.Split(new[] { ';' });

                        foreach (string normal in eachNormal)
                        {
                            bool a = false;
                            string[] normData = normal.Split(new[] { ' ' });
                            List<float> normData2 = new List<float>();
                            foreach (string coord in normData)
                            {
                                if (!string.IsNullOrWhiteSpace(coord))
                                {
                                    normData2.Add(float.Parse(coord));
                                    a = true;
                                }
                            }
                            if (a)
                            {
                                normalList.Add(new Vector3(normData2[0], normData2[1], normData2[2])); // x y z
                            }

                        }
                    }
                }

                //Color data
                {
                    string[] colList = fileText.Split(new[] { 'v' })[1] //after v
                                                 .Split(new[] { 'c' });
                    if (colList.Length == 2)
                    {
                        string thisList = colList[1].Split(new[] { '\n' })[1].Split(new[] { '\n' })[0];
                        string[] eachColor = thisList.Split(new[] { ';' });

                        foreach (string color in eachColor)
                        {
                            bool a = false;
                            string[] colDat = color.Split(new[] { ' ' });
                            List<float> colDat2 = new List<float>();
                            foreach (string coord in colDat)
                            {
                                if (!string.IsNullOrWhiteSpace(coord))
                                {
                                    colDat2.Add(float.Parse(coord));
                                    a = true;
                                }
                            }
                            if (a)
                            {
                                colorList.Add(new Color4(colDat2[0], colDat2[1], colDat2[2], 1.0f)); // r g b a
                            }

                        }
                    }
                }
                
                //Texture data
                {
                    string[] textList = fileText.Split(new[] { 'v' })[1] //after v
                                                 .Split(new[] { 't' });
                    if (textList.Length == 2)
                    {
                        string thisList = textList[1].Split(new[] { '\n' })[1].Split(new[] { '\n' })[0];
                        string[] eachText = thisList.Split(new[] { ';' });

                        foreach (string tex in eachText)
                        {
                            bool a = false;
                            string[] texDat = tex.Split(new[] { ' ' });
                            List<float> texDat2 = new List<float>();
                            foreach (string coord in texDat)
                            {
                                if (!string.IsNullOrWhiteSpace(coord))
                                {
                                    texDat2.Add(float.Parse(coord));
                                    a = true;
                                }
                            }
                            if (a)
                            {
                                textureList.Add(new Vector3(texDat2[0], texDat2[1], texDat2[2])); // r g b a
                            }

                        }
                    }
                }

            }

            string facesText = fileText.Split(new[] { 'v' })[1].Split(new[] { 'f' })[1].Split(new[] { '\n' })[1].Split(new[] { '\n' })[0];
            string[] faces = facesText.Split(new[] { ';' });

            Vector4[] VertexPositions = new Vector4[positionList.Count];
            Vector3[] VertexNormals = normalList.Count > 0 ? new Vector3[normalList.Count] : new Vector3[0];
            Color4[] VertexColors = colorList.Count > 0 ? new Color4[colorList.Count] : new Color4[0];
            Vector3[] VertexTextureCoords = textureList.Count > 0 ? new Vector3[textureList.Count] : new Vector3[0];


            Vertex[] DrawData = new Vertex[(faces.Length * 3)];


            int faceCount = 0;
            foreach (string face in faces)
            {
                string[] eachVert = face.Split(new[] { ' ' });
                foreach (string vert in eachVert)
                {
                    string[] refData = vert.Split(new[] { '/' });
                    if (refData.Length == 3 && !string.IsNullOrEmpty(refData[2]))
                    {
                        int vertexIndex = int.Parse(refData[0]);
                        int textIndex = !string.IsNullOrEmpty(refData[1]) ? int.Parse(refData[1]) : -1;
                        int normIndex = !string.IsNullOrEmpty(refData[2]) ? int.Parse(refData[2]) : -1;

                        Vertex thisVertex = new Vertex();

                        VertexPositions[vertexIndex] = new Vector4(positionList[vertexIndex], 1.0f);
                        thisVertex.Position = VertexPositions[vertexIndex];
                        thisVertex.Color = new Color4(VertexPositions[vertexIndex].X * 0.2f + 0.75f, VertexPositions[vertexIndex].Y * 0.1f + 0.8f, VertexPositions[vertexIndex].Z * 0.35f+0.5f, 1);
                        if (VertexColors.Length != 0)
                        {
                            VertexColors[vertexIndex] = colorList[vertexIndex];
                            thisVertex.Color = colorList[vertexIndex];
                        }

                        if (VertexTextureCoords.Length != 0 && textIndex != -1)
                        {
                            VertexTextureCoords[textIndex] = textureList[textIndex];
                            thisVertex.TextureCoordinates = textureList[textIndex];
                        }

                        if (VertexNormals.Length != 0 && normIndex != -1)
                        {
                            VertexNormals[normIndex] = normalList[normIndex];
                            thisVertex.Normal = normalList[normIndex];
                        }
                        DrawData[faceCount] = thisVertex;
                        
                        faceCount++;
                    }
                }
            }
            


            return Tuple.Create(DrawData, VertexPositions, VertexColors, VertexNormals, VertexTextureCoords);
        }
    }
}
