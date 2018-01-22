using System;
using System.Collections.Generic;
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
            
            //Add game objects
            StudioCamera Camera = new StudioCamera();
            Camera.MoveSensitivity = 0.003f;
            Window.Camera = Camera;


            var land = Window.AddObject(@"Testing\untitled.igwo");
            land.Scale = new Vector3(5, 5, 5);
            land.Position = new Vector3(0, -25, 0);
            

            var Airplane = Window.AddObject(@"Testing\airplane.igwo");
            Airplane.Scale = new Vector3(10, 10, 10);
            Airplane.Position = new Vector3(0, 0, -4);
            
            

            bool[] KeysDown = new bool[] { false, false, false, false, false, false };

            double _time = 0;
            float k;
            void OnUpdateFrame(object sender, FrameEventArgs e)
            {
                _time += e.Time;
                k = (float)_time * 3;
                Airplane.Rotation = new Vector3((float)Math.Cos(_time * 0.75f) * 0.1f,
                                                (float)Math.Sin(_time * 0.5f) * 0.1f + 0.4f,
                                                (float)Math.Sin(_time * 0.5f) * 0.125f);
                Airplane.Position = new Vector3((float)(Math.Sin(k) * 0.01f), (float)(Math.Cos(k * 5f) * 0.025f), -4.0f);
                
                if (KeysDown[0] == true)
                {
                    Camera.Move(0, 0, -1);
                }
                if (KeysDown[1] == true)
                {
                    Camera.Move(1, 0, 0);
                }
                if (KeysDown[2] == true)
                {
                    Camera.Move(0, 0, 1);
                }
                if (KeysDown[3] == true)
                {
                    Camera.Move(-1, 0, 0);
                }
                if (KeysDown[4] == true)
                {
                    Camera.Move(0, -1, 0);
                }
                if (KeysDown[5] == true)
                {
                    Camera.Move(0, 1, 0);
                }
            }




            Vector2 mouseLastPos = new Vector2(0, 0);
            bool isRightMouseDown = false;
            Random pos = new Random();
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
            void KeyDown(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.W)
                {
                    KeysDown[0] = true;
                } else if (e.Key == Key.A)
                {
                    KeysDown[1] = true;
                } else if (e.Key == Key.S)
                {
                    KeysDown[2] = true;
                } else if (e.Key == Key.D)
                {
                    KeysDown[3] = true;
                }
                else if (e.Key == Key.Q)
                {
                    KeysDown[4] = true;
                }
                else if (e.Key == Key.E)
                {
                    KeysDown[5] = true;
                }
            }
            void KeyUp(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.W)
                {
                    KeysDown[0] = false;
                }
                else if (e.Key == Key.A)
                {
                    KeysDown[1] = false;
                }
                else if (e.Key == Key.S)
                {
                    KeysDown[2] = false;
                }
                else if (e.Key == Key.D)
                {
                    KeysDown[3] = false;
                }
                else if (e.Key == Key.Q)
                {
                    KeysDown[4] = false;
                }
                else if (e.Key == Key.E)
                {
                    KeysDown[5] = false;
                }
            }
            void MouseWheel(object sender, MouseWheelEventArgs e)
            {
                Camera.Move(0, 0, -e.Delta / Camera.MoveSensitivity);
            }
            Window.UpdateFrame += OnUpdateFrame;
            Window.MouseDown += MouseDown;
            Window.MouseUp += MouseUp;
            Window.MouseMove += MouseMove;
            Window.KeyDown += KeyDown;
            Window.KeyUp += KeyUp;
            Window.MouseWheel += MouseWheel;

            /*while (true)
            {
                Thread.Sleep(10);
              */  Window.AddObject(@"Testing\MONKEY.igwo").Position = new Vector3(
                    pos.Next(-250, 250)/10f,
                    pos.Next(-250, 250)/10f,
                    pos.Next(-205, 250)/10f
                    );
            //}


            //Exit
            while (Window.Exists) { }
            DebugWriteLine("Shutting down");

        }

    }

}
