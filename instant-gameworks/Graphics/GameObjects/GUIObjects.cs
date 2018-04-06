using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace InstantGameworks.Graphics.GameObjects
{
    public class GuiHolder : Instance
    {

        private static List<int> _shaders = new List<int>() {
            ShaderHandler.CreateShader(@"Shaders\uishader", ShaderType.VertexShader),
            ShaderHandler.CreateShader(@"Shaders\uishaderColor", ShaderType.FragmentShader),
        };

        private float _opacity;

        public override string ClassName { get; } = "GuiHolder";
        public override string Name { get; set; } = "GuiHolder";

        public Vector2 RelativePosition { get; set; } = new Vector2(0, 0);
        public Vector2 RelativeSize { get; set; } = new Vector2(100, 100);
        public Vector2 AbsolutePosition { get; set; } = new Vector2(0, 0);
        public Vector2 AbsoluteSize { get; set; } = new Vector2(0, 0);
        public Color4 Color { get; set; } = Color4.White;
        public float Opacity { get => _opacity; set { _opacity = Math.Clamp(value, 0, 1); } }



        public GuiHolder()
        {
            _objectArray = GL.GenVertexArray();
            _vertexPositionBuffer = GL.GenBuffer();
        }
        public GuiHolder(string name) : base()
        {
            Name = name;
        }




        private struct Vertex
        {
            public static int SizeInBytes = (4 + 4) * 4;
            public Vector4 Position;
            public Color4 Color;
        }
        private int _objectArray;
        private int _vertexPositionBuffer;
        private Vertex[] _vertices;
        public void Render()
        {
            ShaderHandler.CompileShaders(_programId, _shaders); // we want both shaders active at once
            


            GL.BindVertexArray(_objectArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBuffer);
            GL.NamedBufferStorage(_vertexPositionBuffer, _vertices.Length * DrawVertex.SizeInBytes, _vertices, BufferStorageFlags.DynamicStorageBit);

            GL.VertexArrayAttribBinding(_objectArray, 0, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 0);
            GL.VertexArrayAttribFormat(_objectArray, 0, 4, VertexAttribType.Float, false, 0);

            GL.VertexArrayAttribBinding(_objectArray, 1, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 1);
            GL.VertexArrayAttribFormat(_objectArray, 1, 4, VertexAttribType.Float, false, 16);

            GL.VertexArrayVertexBuffer(_objectArray, 0, _vertexPositionBuffer, IntPtr.Zero, DrawVertex.SizeInBytes);
            GL.BindVertexArray(_objectArray);


            GL.BindVertexArray(_objectArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
