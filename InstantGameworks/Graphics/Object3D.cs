using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using InstantGameworks.Graphics.Import;

namespace InstantGameworks.Graphics
{

    public class Object3D : IDisposable
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 RotationalVelocity { get; set; }
        public Color4 Color { get; set; }
        public bool DoRender { get; set; }

        public Vector4[] VertexPositions { get; set; } // x y z
        public Color4[] VertexColors { get; set; } // r g b a
        public Vector3[] VertexNormals { get; set; } // relative x y z
        public Vector3[] VertexTextureCoordinates { get; set; } // u v [w]
        public Vertex[] DrawData { get; set; } // Vertex Vertex Vertex (triangle) sets

        //private static Vertex[] _actualDraw;

        private bool _initialized;
        private readonly int _objectArray;

        private int _vertexPositionBuffer;
        private Matrix4 _modelView;

        private int _vertexCount;

        public Object3D(Vertex[] drawData,
                        Vector4[] vertexPositions,
                        Color4[] vertexColors,
                        Vector3[] vertexNormals, // 
                        Vector3[] vertexTextureCoordinates) // 
        {
            DrawData = drawData;
            VertexPositions = vertexPositions;
            VertexColors = vertexColors;
            VertexNormals = vertexNormals;
            VertexTextureCoordinates = vertexTextureCoordinates;

            _vertexCount = drawData.Length;

            _objectArray = GL.GenVertexArray();
            _vertexPositionBuffer = GL.GenBuffer();

            GL.BindVertexArray(_objectArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBuffer);
            GL.NamedBufferStorage(_vertexPositionBuffer, Vertex.Size * _vertexCount, drawData, BufferStorageFlags.MapWriteBit);

            //Bind attributes
            GL.VertexArrayAttribBinding(_objectArray, 0, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 0);
            GL.VertexArrayAttribFormat(_objectArray, 0, 4, VertexAttribType.Float, false, 0);

            GL.VertexArrayAttribBinding(_objectArray, 1, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 1);
            GL.VertexArrayAttribFormat(_objectArray, 1, 4, VertexAttribType.Float, false, 28);

            GL.VertexArrayVertexBuffer(_objectArray, 0, _vertexPositionBuffer, IntPtr.Zero, Vertex.Size); //set _positionBuffer as part of _vertexArray

            GL.BindVertexArray(_objectArray);

            //Set default internal values
            Position = new Vector3(0, 0, 4.0f);
            Scale = new Vector3(1, 1, 1);
            Rotation = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
            RotationalVelocity = new Vector3(0, 0, 0);
            Color = new Color4(0, 0, 0, 1.0f);
            DoRender = true;

        _initialized = true;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
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
