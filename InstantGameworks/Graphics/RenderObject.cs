using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace InstantGameworks.Graphics
{

    public struct Vertex
    {
        public const int Size = (4 + 4) * 4;

        private readonly Vector4 _position;
        private readonly Color4 _color;

        public Vertex(Vector4 position, Color4 color)
        {
            _position = position;
            _color = color;
        }
    }

    class RenderObject : IDisposable
    {
        private bool _initialized;
        private readonly int _vertexArray;
        private readonly int _buffer;
        private readonly int _vertexCount;

        public RenderObject(Vertex[] vertices)
        {
            _vertexCount = vertices.Length;

            _vertexArray = GL.GenVertexArray();
            _buffer = GL.GenBuffer();
            GL.BindVertexArray(_vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.NamedBufferStorage(_buffer, Vertex.Size * _vertexCount, vertices, BufferStorageFlags.MapWriteBit);

            //Attributes (Position first, then Color)
            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 0, 4, VertexAttribType.Float, false, 0);

            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribFormat(_vertexArray, 1, 4, VertexAttribType.Float, false, 16);

            GL.VertexArrayVertexBuffer(_vertexArray, 0, _buffer, IntPtr.Zero, Vertex.Size);
            
            GL.BindVertexArray(_vertexArray);
            
            _initialized = true;
        }
        public void Render()
        {
            GL.BindVertexArray(_vertexArray);
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
                    GL.DeleteVertexArray(_vertexArray);
                    GL.DeleteBuffer(_buffer);
                    _initialized = false;
                }
            }
        }

    }
}
