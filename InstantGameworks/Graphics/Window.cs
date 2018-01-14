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
        private List<RenderObject> _renderObjectsWhite = new List<RenderObject>();
        private Matrix4 _projectionMatrix;

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
            Icon = new Icon(@"Extra\InstantGameworks.ico");
        }

        //On initial load
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Create program
            _program = ProgramObjectControl.CreateProgram();

            //Adjust render settings
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.LineWidth(1f);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.Enable(EnableCap.DepthTest);
            
            CreateProjection();


            //Create and compile shaders
            int vertexShader = Shaders.CreateShader(@"Graphics\GLSL\vertexshader.glsl", ShaderType.VertexShader);
            int fragmentShader = Shaders.CreateShader(@"Graphics\GLSL\fragmentshader.glsl", ShaderType.FragmentShader);
            _shadersList.Add(vertexShader);
            _shadersList.Add(fragmentShader);

            Shaders.CompileShaders(_program, _shadersList);

            //Create render objects
            Tuple<List<Vertex>, List<Vertex>, List<List<int>>> monkey = LoadOBJ.Import(@"Testing\monkey.obj");
            List<Vertex> vertexData = monkey.Item1;
            List<Vertex> vertexData2 = monkey.Item2;
            List<List<int>> faceData = monkey.Item3;
            foreach (List<int> face in faceData)
            {
                Vertex[] newFace =
                {
                    vertexData[face[0]-1], vertexData[face[1]-1], vertexData[face[2]-1]
                };
                _renderObjects.Add(new RenderObject(newFace));
                Vertex[] newFace2 =
                {
                    vertexData2[face[0]-1], vertexData2[face[1]-1], vertexData2[face[2]-1]
                };
                _renderObjectsWhite.Add(new RenderObject(newFace2));
            }
            
            

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
            Title = "InstantGameworks (OpenGL " + GL.GetString(StringName.Version) + ") FPS: " + (1f/e.Time);



            //Adjust viewport
            GL.Viewport(0, 0, Width, Height);
            
            //Clear frame
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Render frame

            GL.UseProgram(_program);
            GL.UniformMatrix4(2, false, ref _modelView);
            GL.UniformMatrix4(3, false, ref _projectionMatrix);


            foreach (var renderObject in _renderObjects)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                renderObject.Render();
            }
            foreach (var renderObject2 in _renderObjectsWhite)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                renderObject2.Render();
            }
            GL.Flush();
            

            //Update frame
            SwapBuffers();
        }

        //Whenever a frame is updated (SwapBuffers)
        private Matrix4 _modelView;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _time += e.Time;
            var k = (float)_time * 0.375f;
            var r1 = Matrix4.CreateRotationX((float)Math.Cos(_time * 0.75f) * 0.1f);
            var r2 = Matrix4.CreateRotationY((float)Math.Sin(_time * 0.5f) * 0.1f + 0.4f);
            var r3 = Matrix4.CreateRotationZ((float)Math.Sin(_time * 0.5f) * 0.125f);
            var t1 = Matrix4.CreateTranslation((float)(Math.Sin(k) * 0.1f), (float)(Math.Cos(k * 5f) * 0.025f), -0.375f);
            _modelView = r1 * r2 * r3 * t1;
        }

        //Adjust projection
        private void CreateProjection()
        {
            var aspectRatio = (float)Width / Height;
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(90 * ((float)Math.PI / 180f), aspectRatio, 0.1f, 4000f);
        }

        //When window resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            CreateProjection();

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
