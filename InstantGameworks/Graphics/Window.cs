using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.IO;

namespace InstantGameworks.Graphics
{
    public class GameworksWindow : GameWindow
    {
        //Variables
        private int _program;
        private int _vertexArray;
        private List<int> _shadersList = new List<int>();

        //Init
        public GameworksWindow()
            :base(1280, //Width
                 720, //Height
                 OpenTK.Graphics.GraphicsMode.Default, //GraphicsMode
                 "InstantGameworks", //Title
                 GameWindowFlags.Default, //Flags
                 DisplayDevice.Default, //Which monitor
                 4, //Major
                 0, //Minor
                 OpenTK.Graphics.GraphicsContextFlags.Default) //Context flags
        {
            Title += " (OpenGL " + GL.GetString(StringName.Version) + ")";
            CursorVisible = true;
        }

        //On initial load
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Create program
            _program = ProgramObjectControl.CreateProgram();
            
            //Create and compile shaders
            int vertexShader = Shaders.CreateShader(@"Graphics\GLSL\vertexshader.glsl", ShaderType.VertexShader);
            int fragmentShader = Shaders.CreateShader(@"Graphics\GLSL\fragmentshader.glsl", ShaderType.FragmentShader);
            _shadersList.Add(vertexShader);
            _shadersList.Add(fragmentShader);

            Shaders.CompileShaders(_program, _shadersList);

            //Debug
            string DebugInfo = GL.GetProgramInfoLog(_program);
            Console.WriteLine(string.IsNullOrEmpty(DebugInfo) ? "Graphics success" : DebugInfo);

            //Create VAO
            GL.GenVertexArrays(1, out _vertexArray);
            GL.BindVertexArray(_vertexArray);

            //Add closed event
            Closed += OnExit;
        }

        //Whenever a frame is rendered
        private double _time;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _time += e.Time;


            //Adjust viewport
            GL.Viewport(0, 0, Width, Height);
            
            //Clear frame
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Render frame

            GL.VertexAttrib1(0, _time);

            GL.UseProgram(_program);
            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            GL.PointSize(10f);

            GL.Flush();
            

            //Update frame
            SwapBuffers();
        }

        //Whenever a frame is updated (SwapBuffers)
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        //When window resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        //Run code before exit
        private void OnExit(object sender, EventArgs eventArgs)
        {
            Exit();
        }
        
        public override void Exit()
        {
            foreach (int shader in _shadersList)
            {
                Shaders.DeleteShader(shader);
            }
            GL.DeleteVertexArrays(1, ref _vertexArray);
            ProgramObjectControl.DeleteProgram(_program);
            base.Exit();
        }
    }
}
