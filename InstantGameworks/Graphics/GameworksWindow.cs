using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.IO;

using InstantGameworks.Graphics.Import;


namespace InstantGameworks.Graphics
{
    public class GameworksWindow : GameWindow
    {
        //Variables
        private int _program;
        private List<int> _shadersList = new List<int>();
        private List<Object3D> _renderObjects = new List<Object3D>();
        private Matrix4 _projectionMatrix;

        public Object3D Airplane;

        //Init
        public GameworksWindow()
            :base(1280, //Width
                 720, //Height
                 OpenTK.Graphics.GraphicsMode.Default, //GraphicsMode
                 "InstantGameworks", //Title
                 GameWindowFlags.FixedWindow, //Flags
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
            _program = GL.CreateProgram();

            //Adjust render settings
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.LineWidth(0f);
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
            var airplaneVerts = InstantGameworksObject.Import(@"Testing\airplane.igwo");
            Object3D object3D = new Object3D(airplaneVerts.Item1, airplaneVerts.Item2, airplaneVerts.Item3, airplaneVerts.Item4, airplaneVerts.Item5);
            _renderObjects.Add(object3D);
            Airplane = object3D;
            



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
            Title = "InstantGameworks (OpenGL " + GL.GetString(StringName.Version) + ") " + Math.Round(1f/e.Time) + "fps";



            //Adjust viewport
            GL.Viewport(0, 0, Width, Height);
            
            //Clear frame
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Render frame

            GL.UseProgram(_program);
            GL.UniformMatrix4(3, false, ref _projectionMatrix);


            foreach (var renderObject in _renderObjects)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                renderObject.Render();
            }
            

            //Update frame
            SwapBuffers();
        }

        //Whenever a frame is updated (SwapBuffers)
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _time += e.Time;
            var k = (float)_time * 0.375f;
            Airplane.Rotation = new Vector3((float)Math.Cos(_time * 0.75f) * 0.1f,
                                            (float)Math.Sin(_time * 0.5f) * 0.1f + 0.4f,
                                            (float)Math.Sin(_time * 0.5f) * 0.125f);
            Airplane.Position = new Vector3((float)(Math.Sin(k) * 0.01f), (float)(Math.Cos(k * 5f) * 0.025f), -0.375f);
            Airplane.Scale = new Vector3((float)Math.Sin(_time) * 0.25f + 1f, (float)Math.Sin(_time) * 0.25f + 1f, (float)Math.Sin(_time) * 0.25f + 1f);
        }

        //Adjust projection
        private void CreateProjection()
        {
            var aspectRatio = (float)Width / Height;
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(90 * ((float)Math.PI / 180f), aspectRatio, 0.01f, 4000f);
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
            foreach (Object3D obj in _renderObjects)
            {
                obj.Dispose();
            }
            GL.DeleteProgram(_program);
            base.Exit();
        }
    }
}
