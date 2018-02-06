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

        public List<EnableCap> EnableCaps = new List<EnableCap>();

        public Camera Camera = new Camera(); //Current Camera object in use by the program
        public List<Object3D> RenderObjects = new List<Object3D>(); //List of objects in memory


        //Initialization defaults
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
            EnableCaps.Add(EnableCap.DepthTest);
            EnableCaps.Add(EnableCap.Multisample);
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


            //Reset caps
            foreach (EnableCap cap in Enum.GetValues(typeof(EnableCap)))
            {
                GL.Disable(cap);
            }
            foreach (EnableCap cap in EnableCaps)
            {
                GL.Enable(cap);
            }


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











        public Object3D AddObject(string fileName)
        {
            var newObjectData = DataHandler.ReadFile(fileName);
            Object3D newObject = null;
            void Add(object sender, FrameEventArgs e)
            {
                newObject = new Object3D(newObjectData);
                RenderObjects.Add(newObject);
                RenderFrame -= Add;
            }
            RenderFrame += Add;
            while (newObject == null) { }
            return newObject;
        }
    }
}
