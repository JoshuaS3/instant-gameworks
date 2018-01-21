using System;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK;
using OpenTK.Input;
using InstantGameworks.Graphics;
using InstantGameworks.Graphics.Import;

namespace InstantGameworks
{
    class Init
    {

        private static void DebugWriteLine(string output) => Console.WriteLine(Services.SysTime.GetTime() + " " + output);
        
        private static GameworksWindow Window;
        private static IntPtr WindowHandle;

        private static Vector2 WindowSize;
        private static Vector2 WindowPosition;
        private static WindowBorder WindowBorder;
        private static WindowState WindowState;
        private static float RefreshRate;
        private static DisplayDevice WindowDisplay;

        [STAThread]
        private static void OpenWindow()
        {
            DebugWriteLine("Window init");

            Window = new GameworksWindow();
            WindowHandle = Window.WindowInfo.Handle;

            Window.Width = (int)WindowSize.X;
            Window.Height = (int)WindowSize.Y;
            Window.WindowBorder = WindowBorder;
            Window.WindowState = WindowState;
            Window.X = (int)WindowPosition.X;
            Window.Y = (int)WindowPosition.Y;
            DebugWriteLine("Window success");

            Window.Run(RefreshRate, RefreshRate);
            
        }
        
        public static void Main()
        {
            Console.Title = "Instant Gameworks";
            Console.WriteLine("Instant Gameworks (c)2018");


            //Main
            DebugWriteLine("Init");

            //Set window settings
            DisplayDevice defaultDisplay = DisplayDevice.Default;
            WindowDisplay = defaultDisplay;
            RefreshRate = 0;
            WindowSize = new Vector2(1920, 1080);
            WindowPosition = new Vector2(0, 0);
            WindowBorder = WindowBorder.Fixed;
            WindowState = WindowState.Fullscreen;

            //Create window
            ThreadStart GameThread = new ThreadStart(OpenWindow);
            Thread RunGame = new Thread(GameThread);
            RunGame.Start();

            //Wait for window
            while (Window == null) { } //Wait until window initiates

            StudioCamera Camera = new StudioCamera();
            Window.Load += (sender, e) =>
            {
                Object3D Airplane = Window.AddObject(InstantGameworksObject.Import(@"Testing\MONKEY.igwo"));
                Airplane.Scale = new Vector3(1, 1, 1);
                Airplane.Position = new Vector3(0, 0, -4);

                Object3D land = Window.AddObject(InstantGameworksObject.Import(@"Testing\untitled.igwo"));
                land.Position = new Vector3(0, -5, 0);
                Window.RenderObjects.Add(land);

                Window.Camera = Camera;
            };




            double _time = 0;
            float k;
            void OnUpdateFrame(object sender, FrameEventArgs e)
            {
                _time += e.Time;
                k = (float)_time * 3;
                Window.RenderObjects[0].Rotation = new Vector3((float)Math.Cos(_time * 0.75f) * 0.1f,
                                                (float)Math.Sin(_time * 0.5f) * 0.1f + 0.4f,
                                                (float)Math.Sin(_time * 0.5f) * 0.125f);
                Window.RenderObjects[0].Position = new Vector3((float)(Math.Sin(k) * 0.01f), (float)(Math.Cos(k * 5f) * 0.025f), -4.0f);
            }



            Vector2 mouseLastPos = new Vector2(0, 0);
            bool isRightMouseDown = false;
            void MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (e.Button == MouseButton.Right)
                {
                    if (!isRightMouseDown)
                    {
                        isRightMouseDown = true;
                        mouseLastPos = new Vector2(e.X, e.Y);
                    }
                }
            }
            void MouseUp(object sender, MouseButtonEventArgs e)
            {
                if (e.Button == MouseButton.Right)
                {
                    isRightMouseDown = false;
                }
            }
            void MouseMove(object sender, MouseMoveEventArgs e)
            {
                if (Window.Focused && isRightMouseDown)
                {
                    Camera.AddRotation(e.XDelta, e.YDelta);
                }
            }
            void KeyDown(object sender, KeyPressEventArgs e)
            {
                switch (char.ToLower(e.KeyChar))
                {
                    case 'w':
                        Camera.Move(0, 0, -1);
                        break;
                    case 'a':
                        Camera.Move(1, 0, 0);
                        break;
                    case 's':
                        Camera.Move(0, 0, 1);
                        break;
                    case 'd':
                        Camera.Move(-1, 0, 0);
                        break;
                    case 'q':
                        Camera.Move(0, 1, 0);
                        break;
                    case 'e':
                        Camera.Move(0, -1, 0);
                        break;
                }
            }
            Window.UpdateFrame += OnUpdateFrame;
            Window.MouseDown += MouseDown;
            Window.MouseUp += MouseUp;
            Window.MouseMove += MouseMove;
            Window.KeyPress += KeyDown;

            
            //Exit
            DebugWriteLine("Shutting down");

        }

    }

}
