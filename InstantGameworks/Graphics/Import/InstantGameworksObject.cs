using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace InstantGameworks.Graphics.Import
{
    public struct InstantGameworksObject
    {
        public Position[] Positions;
        public TextureCoordinates[] TextureCoordinates;
        public Position[] Normals;

        public Face[] Faces;
    }

    public struct Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public Position(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct TextureCoordinates
    {

        public float U { get; set; }
        public float V { get; set; }
        public float W { get; set; }
        TextureCoordinates(float u, float v)
        {
            U = u;
            V = v;
            W = 0;
        }
        TextureCoordinates(float u, float v, float w)
        {
            U = u;
            V = v;
            W = w;
        }
    }

    public struct Vertex
    {
        public int PositionIndex; //Where in the Positions list can I find this data? (gets rid of repitition)
        public int TextureCoordinatesIndex;
        public int NormalIndex;
    }

    public struct Face
    {
        public Vertex[] Vertices;
    }

    public class DataHandler
    {
        public static void WriteFile(string fileLocation, InstantGameworksObject objectData)
        {
            Stream _fileStream = new FileStream(fileLocation, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter _binaryWriter = new BinaryWriter(_fileStream);


            _binaryWriter.Write('P'); //indicate beginning of positions array
            _binaryWriter.Write(objectData.Positions.Length);
            foreach (Position position in objectData.Positions)
            {
                _binaryWriter.Write(position.X);
                _binaryWriter.Write(position.Y);
                _binaryWriter.Write(position.Z);
            }

            _binaryWriter.Write('T'); //indicate beginning of texture coordinates array
            _binaryWriter.Write(objectData.TextureCoordinates.Length);
            foreach (TextureCoordinates textCoord in objectData.TextureCoordinates)
            {
                _binaryWriter.Write(textCoord.U);
                _binaryWriter.Write(textCoord.V);
                _binaryWriter.Write(textCoord.W);
            }

            _binaryWriter.Write('N'); //indicate beginning of normals array
            _binaryWriter.Write(objectData.Normals.Length);
            foreach (Position normal in objectData.Normals)
            {
                _binaryWriter.Write(normal.X);
                _binaryWriter.Write(normal.Y);
                _binaryWriter.Write(normal.Z);
            }

            _binaryWriter.Write('F'); //indicate beginning of normals array
            _binaryWriter.Write(objectData.Faces.Length);
            foreach (Face face in objectData.Faces)
            {
                foreach (Vertex vertex in face.Vertices)
                {
                    _binaryWriter.Write(vertex.PositionIndex);
                    _binaryWriter.Write(vertex.TextureCoordinatesIndex);
                    _binaryWriter.Write(vertex.NormalIndex);
                }
            }


            _binaryWriter.Close();
            _fileStream.Close();
        }

        public static InstantGameworksObject ReadFile(string fileLocation)
        {
            InstantGameworksObject objectData = new InstantGameworksObject();

            Stream _fileStream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader _binaryReader = new BinaryReader(_fileStream);

            List<Position> positionData = new List<Position>();
            List<TextureCoordinates> textureData = new List<TextureCoordinates>();
            List<Position> normalData = new List<Position>();
            List<Face> faceData = new List<Face>();

            int arrayLength;

            _binaryReader.ReadChar(); //'P' for position
            arrayLength = _binaryReader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                Position newPos = new Position();
                newPos.X = _binaryReader.ReadSingle();
                newPos.Y = _binaryReader.ReadSingle();
                newPos.Z = _binaryReader.ReadSingle();
                positionData.Add(newPos);
            }

            _binaryReader.ReadChar(); //'T' for texture coordinates
            arrayLength = _binaryReader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                TextureCoordinates newUVW = new TextureCoordinates();
                newUVW.U = _binaryReader.ReadSingle();
                newUVW.V = _binaryReader.ReadSingle();
                newUVW.W = _binaryReader.ReadSingle();
                textureData.Add(newUVW);
            }

            _binaryReader.ReadChar(); //'N' for normals
            arrayLength = _binaryReader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                Position newPos = new Position();
                newPos.X = _binaryReader.ReadSingle();
                newPos.Y = _binaryReader.ReadSingle();
                newPos.Z = _binaryReader.ReadSingle();
                normalData.Add(newPos);
            }

            _binaryReader.ReadChar(); //'F' for face
            arrayLength = _binaryReader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                Face newFace = new Face();
                newFace.Vertices = new Vertex[3];
                for (int x = 0; x < 3; x++) //for each vertex
                {
                    Vertex newVertex = new Vertex();
                    newVertex.PositionIndex = _binaryReader.ReadInt32();
                    newVertex.TextureCoordinatesIndex = _binaryReader.ReadInt32();
                    newVertex.NormalIndex = _binaryReader.ReadInt32();
                    newFace.Vertices[x] = newVertex;
                }
                faceData.Add(newFace);
            }

            objectData.Positions = positionData.ToArray();
            objectData.TextureCoordinates = textureData.ToArray();
            objectData.Normals = normalData.ToArray();
            objectData.Faces = faceData.ToArray();


            _binaryReader.Close();
            _fileStream.Close();

            return objectData;
        }
    }
}