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

using InstantGameworksObject;
using InstantGameworks.Graphics.GameObjects;


namespace InstantGameworks.Graphics
{
    public class GameworksWindow : GameWindow
    {
        // Variables
        private int _programId;
        private List<int> _shadersList = new List<int>();
        private Matrix4 _cameraMatrix;

        private List<DirectionalLight> _directionalLights = new List<DirectionalLight>(8); // more than 8 is a just mess
        private List<PointLight> _pointLights = new List<PointLight>(512); // great doubt that a scene would consist of more than 512 lights

        public List<EnableCap> EnableCaps { get; } = new List<EnableCap>(); // EnableCaps currently in use
        public Camera Camera { get; set; } = new Camera(); // Current Camera object in use by the program
        public List<Object3D> RenderObjects { get; } = new List<Object3D>(); // The program's current render queue


        // Initialization defaults
        public GameworksWindow()
            :base(1280,
                  720,
                  new GraphicsMode(32, 8, 0, 8),
                  "InstantGameworks",
                  GameWindowFlags.FixedWindow,
                  DisplayDevice.Default,
                  4,
                  0,
                  GraphicsContextFlags.Default)
        {
            Title += " (OpenGL " + GL.GetString(StringName.Version) + ")";
            CursorVisible = true;
            Icon = new Icon(@"Extra\InstantGameworks.ico");
            EnableCaps.Add(EnableCap.DepthTest);
            EnableCaps.Add(EnableCap.Multisample);
            EnableCaps.Add(EnableCap.LineSmooth);
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Create program
            _programId = GL.CreateProgram();

            // Adjust render settings
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);

            // Create and compile shaders
            int vertexShader = Shaders.CreateShader(@"Shaders\vertexshader.glsl", ShaderType.VertexShader);
            int fragmentShader = Shaders.CreateShader(@"Shaders\fragmentshader.glsl", ShaderType.FragmentShader);
            _shadersList.Add(vertexShader);
            _shadersList.Add(fragmentShader);

            Shaders.CompileShaders(_programId, _shadersList);
            
            // Debug
            string DebugInfo = GL.GetProgramInfoLog(_programId);
            Logging.LogEvent(string.IsNullOrEmpty(DebugInfo) ? "Graphics success" : DebugInfo);

            // Add closed event
            Closed += OnExit;
        }

        // Whenever a frame is rendered
        private double _time;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _time += e.Time;
            Title = "InstantGameworks (OpenGL " + GL.GetString(StringName.Version) + ") " + Math.Round(1f / e.Time) + "fps";
            
            // Adjust viewport
            GL.Viewport(0, 0, Width, Height);

            // Clear frame
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            // Reset caps
            foreach (EnableCap cap in Enum.GetValues(typeof(EnableCap)))
            {
                GL.Disable(cap);
            }
            foreach (EnableCap cap in EnableCaps)
            {
                GL.Enable(cap);
            }

            // Render frame
            GL.UseProgram(_programId);

            // Update camera
            Camera.AspectRatio = (float)Width / Height;
            Camera.Update();
            _cameraMatrix = Camera.PerspectiveMatrix;
            GL.UniformMatrix4(3, false, ref _cameraMatrix);
            
            
            
            foreach (var renderObject in RenderObjects)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Uniform4(1, renderObject.Color);
                renderObject.Render();
            }
            
            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
        
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
            GL.DeleteProgram(_programId);
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

        public PointLight AddPointLight()
        {
            var newObject = new PointLight();
            _pointLights.Add(newObject);
            return newObject;
        }

        public DirectionalLight AddDirectionalLight()
        {
            var newObject = new DirectionalLight();
            _directionalLights.Add(newObject);
            return newObject;
        }

        public void RemovePointLight(PointLight pointLight)
        {
            _pointLights.Remove(pointLight);
        }

        public void RemoveDirectionalLight(DirectionalLight directionalLight)
        {
            _directionalLights.Remove(directionalLight);
        }
    }
}
