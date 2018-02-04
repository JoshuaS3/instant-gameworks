using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using InstantGameworks.Graphics.Import;

namespace InstantGameworks.Graphics
{
    public class Instance : IDisposable
    {
        private List<string> _inherits = new List<string>() { };

        public bool Archivable { get; set; } = true;
        public virtual string ClassName { get; } = "Instance";
        public virtual string Name { get; set; } = "Instance";


        public override string ToString()
        {
            return Name;
        }

        public bool Inherits(string className)
        {
            if (_inherits.Contains(className))
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Name = null;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }
    }

    public class LightSource : Instance
    {
        private List<string> _inherits = new List<string>() { "Instance" };

        public override string ClassName { get; } = "LightSource";
        public override string Name { get; set; } = "LightSource";

        public virtual Color4 Color { get; set; }
        public virtual float Intensity { get; set; }
        public virtual bool Enabled { get; set; }
    }

    public class PointLight : LightSource
    {
        private List<string> _inherits = new List<string>() { "Instance", "LightSource" };

        public override string ClassName { get; } = "PointLight";
        public override string Name { get; set; } = "PointLight";

        public override Color4 Color { get; set; }
        public override float Intensity { get; set; }
        public override bool Enabled { get; set; }

        public float Radius { get; set; }

        public PointLight(Color4 color, float intensity, float radius)
        {
            Color = color;
            Intensity = intensity;
            Radius = radius;
            Enabled = true;
        }

    }






    struct DrawVertex
    {
        public static int SizeInBytes = (4 + 4 + 3 + 3) * 4;
        public Vector4 position;
        public Color4 color;
        public Vector3 normal;
        public Vector3 texture;
    }

    public class Object3D : Instance
    {
        private List<string> _inherits = new List<string>() { "Instance" };

        public override string ClassName { get; } = "Object3D";
        public override string Name { get; set; } = "Object3D";


        private Vector3 _position;
        private Vector3 _scale;
        private Vector3 _rotation;
        private Vector3 _velocity;
        private Vector3 _rotationalVelocity;
        private Color4 _color;
        private bool _doRender;
        private int _vertexCount;
        private Position[] _vertexPositions;
        private Position[] _vertexNormals;
        private TextureCoordinates[] _vertexTextureCoordinates;
        private Face[] _faces;

        private DrawVertex[] _sortedVertices;


        public Vector3 Position { get => _position; set => _position = value; }
        public Vector3 Scale { get => _scale; set => _scale = value; }
        public Vector3 Rotation { get => _rotation; set => _rotation = value; }
        public Vector3 Velocity { get => _velocity; set => _velocity = value; }
        public Vector3 RotationalVelocity { get => _rotationalVelocity; set => _rotationalVelocity = value; }
        public Color4 Color {
            get {
                return _color;
            }
            set {
                _color = value;
                _updateColor(value); //reset VBO
            }
        }
        public bool DoRender { get => _doRender; set => _doRender = value; }
        public int VertexCount { get => _vertexCount; set => _vertexCount = value; }


        public Position[] VertexPositions { get => _vertexPositions; set => _vertexPositions = value; } // x y z
        public Position[] VertexNormals { get => _vertexNormals; set => _vertexNormals = value; } // relative x y z
        public TextureCoordinates[] VertexTextureCoordinates { get => _vertexTextureCoordinates; set => _vertexTextureCoordinates = value; } // u v [w]
        public Face[] Faces { get => _faces; set => _faces = value; }

        private bool _initialized;
        private int _objectArray;

        private int _vertexPositionBuffer;
        private Matrix4 _modelView;

        public Object3D(InstantGameworksObject data)
        {
            _init(data.Positions, data.TextureCoordinates, data.Normals, data.Faces);
        }

        internal void _init(Position[] _vertPos, TextureCoordinates[] _vertText, Position[] _vertNorm, Face[] _face)
        {
            //Set default internal values
            _position = new Vector3(0, 0, 0);
            _scale = new Vector3(1, 1, 1);
            _rotation = new Vector3(0, 0, 0);
            _velocity = new Vector3(0, 0, 0);
            _rotationalVelocity = new Vector3(0, 0, 0);
            _color = new Color4(255, 255, 255, 255);
            _doRender = true;

            _vertexPositions = _vertPos;
            _vertexTextureCoordinates = _vertText;
            _vertexNormals = _vertNorm;
            _faces = _face;
            
            _sortData();
            _vertexCount = Faces.Length * 3;

            _objectArray = GL.GenVertexArray();

            _vertexPositionBuffer = GL.GenBuffer();


            //Position buffer

            _updateVBO();

            _initialized = true;
        }
        private void _updateVBO()
        {
            GL.BindVertexArray(_objectArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBuffer);
            GL.NamedBufferStorage(_vertexPositionBuffer, _sortedVertices.Length * DrawVertex.SizeInBytes, _sortedVertices, BufferStorageFlags.DynamicStorageBit);

            GL.VertexArrayAttribBinding(_objectArray, 0, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 0);
            GL.VertexArrayAttribFormat(_objectArray, 0, 4, VertexAttribType.Float, false, 0);

            GL.VertexArrayAttribBinding(_objectArray, 1, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 1);
            GL.VertexArrayAttribFormat(_objectArray, 1, 4, VertexAttribType.Float, false, 16);

            GL.VertexArrayVertexBuffer(_objectArray, 0, _vertexPositionBuffer, IntPtr.Zero, DrawVertex.SizeInBytes); //set _positionBuffer as part of _vertexArray



            GL.BindVertexArray(_objectArray);
        }
        
        private void _updateColor(Color4 color)
        {
            _color = color;
            _sortData();
            GL.BindVertexArray(_objectArray);
            GL.NamedBufferSubData(_vertexPositionBuffer, IntPtr.Zero, _sortedVertices.Length * DrawVertex.SizeInBytes, _sortedVertices);
        }

        private void _sortData()
        {
            _sortedVertices = new DrawVertex[_faces.Length * 3];

            int count = 0;
            foreach (Face face in _faces)
            {
                foreach (Vertex vertex in face.Vertices)
                {
                    DrawVertex nv = new DrawVertex();

                    Position thisPos = _vertexPositions[vertex.PositionIndex - 1];
                    nv.position = new Vector4(thisPos.X, thisPos.Y, thisPos.Z, 1);


                    nv.color = Color;

                    /*
                    if (vertex.NormalIndex != -1)
                    {
                        Position thisNorm = VertexNormals[vertex.NormalIndex - 1];
                        nv.normal = new Vector3(thisNorm.X, thisNorm.Y, thisNorm.Z);
                    }
                    else
                    {
                        nv.normal = new Vector3(0, 0, 0);
                    }


                    if (vertex.TextureCoordinatesIndex != -1)
                    {
                        TextureCoordinates thisTexture = VertexTextureCoordinates[vertex.TextureCoordinatesIndex - 1];
                        nv.texture = new Vector3(thisTexture.U, thisTexture.V, thisTexture.W);
                    }
                    else
                    {
                        nv.texture = new Vector3(0, 0, 0);
                    }*/


                    _sortedVertices[count] = nv;

                    count++;
                }
            }
        }

        public void Render()
        {
            _modelView = Matrix4.CreateRotationX(Rotation.X) *
                         Matrix4.CreateRotationY(Rotation.Y) *
                         Matrix4.CreateRotationZ(Rotation.Z) *
                         Matrix4.CreateScale(Scale.X, Scale.Y, Scale.Z) *
                         Matrix4.CreateTranslation(Position.X, Position.Y, Position.Z);
            GL.UniformMatrix4(2, false, ref _modelView);

            GL.BindVertexArray(_objectArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_initialized)
                {
                    GL.DeleteVertexArray(_objectArray);
                    GL.DeleteBuffer(_vertexPositionBuffer);
                    _initialized = false;
                }
            }
        }
    }
}
