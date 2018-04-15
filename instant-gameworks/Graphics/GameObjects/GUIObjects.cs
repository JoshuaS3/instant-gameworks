using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace InstantGameworks.Graphics.GameObjects
{
    public class GuiHolder : Instance, IDisposable
    {

        public static List<int> _shaders = new List<int>() {
            ShaderHandler.CreateShader(@"Shaders\uishader.glsl", ShaderType.VertexShader),
            ShaderHandler.CreateShader(@"Shaders\uishaderColor.glsl", ShaderType.FragmentShader),
        };

        private float _opacity;
        private Vector2 _absolutePosition = new Vector2(0, 0);
        private Vector2 _absoluteSize = new Vector2(0.01f, 0.01f);
        

        public override string ClassName { get; } = "GuiHolder";
        public override string Name { get; set; } = "GuiHolder";
        
        public Vector2 AbsolutePosition { get => _absolutePosition; set { _absolutePosition = value; _updateSizeAndPosition(); } }
        public Vector2 AbsoluteSize { get => _absoluteSize; set { _absoluteSize = value; _updateSizeAndPosition(); } }
        public Color4 Color { get; set; } = Color4.White;
        public float Opacity { get => _opacity; set { _opacity = Math.Clamp(value, 0, 1); } }


        public delegate void RenderEvent(object sender, EventArgs e);
        public event RenderEvent OnRender = delegate { };


        public static int _pid = GL.CreateProgram();

        public GuiHolder()
        {
            _objectArray = GL.GenVertexArray();
            _vertexPositionBuffer = GL.GenBuffer();
            
            _updateSizeAndPosition();
            
            _initialized = true;
        }
        public GuiHolder(string name) : base()
        {
            Name = name;
        }

        private bool _initialized = false;
        private readonly int _objectArray;
        private readonly int _vertexPositionBuffer;
        private Vector4[] _vertices;

        public void _updateBuffers()
        {
            GL.BindVertexArray(_objectArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBuffer);
            GL.NamedBufferData(_vertexPositionBuffer, _vertices.Length * Vector4.SizeInBytes, _vertices, BufferUsageHint.DynamicDraw);
            
            GL.VertexArrayAttribBinding(_objectArray, 0, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 0);
            GL.VertexArrayAttribFormat(_objectArray, 0, 4, VertexAttribType.Float, false, 0);

            GL.VertexArrayVertexBuffer(_objectArray, 0, _vertexPositionBuffer, IntPtr.Zero, Vector4.SizeInBytes);
        }

        public void _updateSizeAndPosition()
        {
            Vector2 offset = new Vector2(1f, -1f);
            Vector2 adjustedPosition = (_absolutePosition + offset) / 2f;
            Vector2 adjustedSize = _absoluteSize * 2f;

            Vector2 _topLeft = adjustedPosition;
            Vector2 _topRight = adjustedPosition + new Vector2(adjustedSize.X, 0);
            Vector2 _bottomLeft = adjustedPosition + new Vector2(0, -adjustedSize.Y);
            Vector2 _bottomRight = adjustedPosition + new Vector2(adjustedSize.X, -adjustedSize.Y);
            _vertices = new Vector4[]
            {
                new Vector4(_topLeft.X, _topLeft.Y, 0, 1),
                new Vector4(_topRight.X, _topRight.Y, 0, 1),
                new Vector4(_bottomLeft.X, _bottomLeft.Y, 0, 1),

                new Vector4(_topRight.X, _topRight.Y, 0, 1),
                new Vector4(_bottomLeft.X, _bottomLeft.Y, 0, 1),
                new Vector4(_bottomRight.X, _bottomRight.Y, 0, 1),
            };
        }

        public void Render()
        {
            OnRender(this, EventArgs.Empty);
            _updateBuffers();
            GL.Uniform4(1, Color);
            GL.BindVertexArray(_objectArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length);
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


