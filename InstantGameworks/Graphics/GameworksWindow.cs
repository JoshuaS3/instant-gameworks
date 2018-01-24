using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
        private Matrix4 _cameraMatrix;


        public Camera Camera = new Camera();
        public List<Object3D> RenderObjects = new List<Object3D>();


        //Init
        public GameworksWindow()
            :base(1280, //Width
                  720, //Height
                  new GraphicsMode(32, 8, 0, 8), //GraphicsMode
                  "InstantGameworks", //Title
                  GameWindowFlags.FixedWindow, //Flags
                  DisplayDevice.Default, //Which monitor
                  4, //Major
                  0, //Minor
                  GraphicsContextFlags.Default) //Context flags
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
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);


            //Create and compile shaders
            int vertexShader = Shaders.CreateShader(@"Graphics\GLSL\vertexshader.glsl", ShaderType.VertexShader);
            int fragmentShader = Shaders.CreateShader(@"Graphics\GLSL\fragmentshader.glsl", ShaderType.FragmentShader);
            _shadersList.Add(vertexShader);
            _shadersList.Add(fragmentShader);

            Shaders.CompileShaders(_program, _shadersList);
            



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
            _time += e.Time;
            Title = "InstantGameworks (OpenGL " + GL.GetString(StringName.Version) + ") " + Math.Round(1f / e.Time) + "fps";


            //Adjust viewport
            GL.Viewport(0, 0, Width, Height);

            //Clear frame
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Render frame

            GL.UseProgram(_program);



            Camera.AspectRatio = (float)Width / Height;
            Camera.Update();
            _cameraMatrix = Camera.PerspectiveMatrix;
            GL.UniformMatrix4(3, false, ref _cameraMatrix);

            var currentObjects = RenderObjects.ToList();
            foreach (var renderObject in currentObjects)
            {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                renderObject.Render();
            }


            //Update frame
            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }


        public Object3D AddObject(string fileName)
        {
            var newObjectData = InstantGameworksObject.Import(fileName);
            Object3D newObject = null;
            void Add(object sender, FrameEventArgs e)
            {
                newObject = new Object3D(newObjectData);
                RenderFrame -= Add;
            }
            RenderFrame += Add;
            while (newObject == null) { }
            RenderObjects.Add(newObject);
            return newObject;
        }
        public Object3D AddObject(Vertex[] drawData,
                                  Vector4[] vertexPositions,
                                  Color4[] vertexColors,
                                  Vector3[] vertexNormals, // 
                                  Vector3[] vertexTextureCoordinates)
        {
            var newObjectData = new Tuple<Vertex[], Vector4[], Color4[], Vector3[], Vector3[]>(drawData, vertexPositions, vertexColors, vertexNormals, vertexTextureCoordinates);

            Object3D newObject = null;
            void Add(object sender, FrameEventArgs e)
            {
                newObject = new Object3D(newObjectData);
                UpdateFrame -= Add;
            }
            UpdateFrame += Add;
            while (newObject == null) { }
            RenderObjects.Add(newObject);
            return newObject;
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
            foreach (Object3D obj in RenderObjects)
            {
                obj.Dispose();
            }
            GL.DeleteProgram(_program);
            base.Exit();
        }
    }
}
