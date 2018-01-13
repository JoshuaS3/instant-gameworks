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
        private List<int> _shadersList = new List<int>();
        private List<RenderObject> _renderObjects = new List<RenderObject>();

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

            //Create render objects
            Tuple<List<Vertex>, List<List<int>>> monkey = LoadOBJ.Import(@"Testing\monkey.obj");
            List<Vertex> vertexData = monkey.Item1;
            List<List<int>> faceData = monkey.Item2;
            foreach (List<int> face in faceData)
            {
                Vertex[] newFace =
                {
                    vertexData[face[0]-1], vertexData[face[1]-1], vertexData[face[2]-1]
                };
                _renderObjects.Add(new RenderObject(newFace));
            }
            
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.LineWidth(5f);
            //GL.PatchParameter(PatchParameterInt.PatchVertices, 3);

            //Debug
            string DebugInfo = GL.GetProgramInfoLog(_program);
            Console.WriteLine(string.IsNullOrEmpty(DebugInfo) ? "Graphics success" : DebugInfo);

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
            GL.Enable(EnableCap.DepthTest);

            //Render frame

            GL.UseProgram(_program);
            GL.UniformMatrix4(20, false, ref _modelView);
            foreach (var renderObject in _renderObjects)
            {
                renderObject.Render();
            }
            //GL.Flush();
            

            //Update frame
            SwapBuffers();
        }

        //Whenever a frame is updated (SwapBuffers)
        private Matrix4 _modelView;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _time += e.Time;
            var k = (float)_time * 0.05f;
            var r1 = Matrix4.CreateRotationX(k * 3.0f);
            var r2 = Matrix4.CreateRotationY(k * 3.0f);
            var r3 = Matrix4.CreateRotationZ(k * 1.0f);
            var t1 = Matrix4.CreateTranslation((float)(Math.Sin(k * 5f) * 0.5f), (float)(Math.Cos(k * 5f) * 0.5f), 0f);
            _modelView = r1 * t1 * r2 * r3;
        }

        //When window resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.UniformMatrix4(20, false, ref projection);

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
            foreach (RenderObject obj in _renderObjects)
            {
                obj.Dispose();
            }
            ProgramObjectControl.DeleteProgram(_program);
            base.Exit();
        }
    }
}
