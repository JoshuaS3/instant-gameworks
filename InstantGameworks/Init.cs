using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK;
using OpenTK.Input;
using InstantGameworks.Services;
using InstantGameworks.Graphics;
using InstantGameworks.Graphics.Import;

namespace InstantGameworks
{
    class Init
    {
        private static void DebugWriteLine(string output) => Console.WriteLine(SysTime.GetTime() + " " + output);
        
        private static GameworksWindow GameWindow;
        private static IntPtr GameWindowHandle;

        private static Vector2 GameWindowSize;
        private static Vector2 GameWindowPosition;
        private static WindowBorder GameWindowBorder;
        private static WindowState GameWindowState;
        private static float GameWindowRefreshRate;

        [STAThread]
        private static void CreateGameworksWindow()
        {

            GameWindow = new GameworksWindow();
            GameWindowHandle = GameWindow.WindowInfo.Handle;

            GameWindow.Width = (int)GameWindowSize.X;
            GameWindow.Height = (int)GameWindowSize.Y;
            GameWindow.WindowBorder = GameWindowBorder;
            GameWindow.WindowState = GameWindowState;
            GameWindow.X = (int)GameWindowPosition.X;
            GameWindow.Y = (int)GameWindowPosition.Y;

            GameWindow.VSync = VSyncMode.On;
            GameWindow.Run(GameWindowRefreshRate, GameWindowRefreshRate);
        }

        [STAThread]
        public static void Main()
        {
            Console.Title = "Instant Gameworks";
            Console.WriteLine("Instant Gameworks (c)  2018");
            
            DebugWriteLine("Main thread startup");
            
            // Set window settings
            DisplayDevice DefaultDisplay = DisplayDevice.Default;
            GameWindowRefreshRate = DefaultDisplay.RefreshRate;
            GameWindowSize = new Vector2(1280, 720); 
            GameWindowPosition = new Vector2(100, 50);
            GameWindowBorder = WindowBorder.Fixed;
            GameWindowState = WindowState.Normal;

            // Create window
            DebugWriteLine("Initializing GameworksWindow");

            ThreadStart GameThread = new ThreadStart(CreateGameworksWindow);
            Thread RunGame = new Thread(GameThread);
            RunGame.Start();

            // Wait for window to initialize
            while (GameWindow == null) { }
            while (!GameWindow.Exists) { }

//            ConsoleApp.HideConsole();




            //
            //
            //
            // Game logic
            //
            //
            //
            
            // Initialize camera
            StudioCamera Camera = new StudioCamera();
            Camera.MoveSensitivity = 0.08f;
            GameWindow.Camera = Camera;
            
            
            var Airplane = GameWindow.AddObject(@"Testing\airplane.igwo");
            Airplane.Scale = new Vector3(10, 10, 10);
            Airplane.Position = new Vector3(0, 0, -4);

            var Land = GameWindow.AddObject(@"Testing\land.igwo");
            Land.Scale = new Vector3(10, 30, 10);
            Land.Position = new Vector3(0, -20, 0);



            double _time = 0;
            void OnUpdateFrameTimer(object sender, FrameEventArgs e)
            {
                _time += e.Time;
            }
            void ObjectUpdateFrame(object sender, FrameEventArgs e)
            {
                Airplane.Rotation = new Vector3((float)Math.Cos(_time * 0.75f) * 0.1f,
                                                (float)Math.Sin(_time * 0.5f) * 0.1f + 0.4f,
                                                (float)Math.Sin(_time * 0.5f) * 0.125f);
                Airplane.Position = new Vector3((float)(Math.Sin(_time) * 0.01f), (float)(Math.Cos(_time * 5f) * 0.025f), -4.0f);
                
                float hue = ((float)_time * 0.1f) % 1f;
                var color = OpenTK.Graphics.Color4.FromHsv(new Vector4(hue, 0.5f, 0.5f, 0.5f));
                //Airplane.Color = color;
            }

            // Camera implementation
            Dictionary<Key, bool> KeysDown = new Dictionary<Key, bool>() { [Key.W] = false, [Key.A] = false, [Key.S] = false, [Key.D] = false };

            Vector2 LastMousePosition = new Vector2(0, 0);
            bool IsRightMouseDown = false;
            bool IsSettingMousePosition = false;
            void CameraUpdateFrame(object sender, FrameEventArgs e)
            {
                if (IsRightMouseDown) //if the camera is being rotated, keep the mouse in the same spot on the screen
                {
                    IsSettingMousePosition = true;
                    Mouse.SetPosition(LastMousePosition.X + GameWindow.X + 8, LastMousePosition.Y + GameWindow.Y + 31);
                }

                if (KeysDown[Key.W] == true)
                {
                    Camera.Move(0, 0, -1);
                }
                if (KeysDown[Key.A] == true)
                {
                    Camera.Move(1, 0, 0);
                }
                if (KeysDown[Key.S] == true)
                {
                    Camera.Move(0, 0, 1);
                }
                if (KeysDown[Key.D] == true)
                {
                    Camera.Move(-1, 0, 0);
                }
            }

            
            void MouseDown(object sender, MouseButtonEventArgs e) //is triggered whenever a mouse button is pressed
            {
                if (e.Button == MouseButton.Right)
                {
                    if (!IsRightMouseDown)
                    {
                        IsRightMouseDown = true;
                        LastMousePosition = new Vector2(e.X, e.Y);
                    }
                }
            }
            void MouseUp(object sender, MouseButtonEventArgs e) //triggered whenever a mouse button is released
            {
                if (e.Button == MouseButton.Right)
                {
                    IsRightMouseDown = false;
                }
            }
            void MouseMove(object sender, MouseMoveEventArgs e) //triggered whenever the mouse is moved
            {
                if (GameWindow.Focused && IsRightMouseDown && !IsSettingMousePosition) //if the window's in focus, the RMB is down, and the program isn't doing it
                {
                    Camera.AddRotation(e.XDelta, e.YDelta);
                }
                IsSettingMousePosition = false;
            }
            void KeyDown(object sender, KeyboardKeyEventArgs e)
            {
                switch (e.Key)
                {
                    case Key.W:
                        KeysDown[Key.W] = true;
                        break;
                    case Key.A:
                        KeysDown[Key.A] = true;
                        break;
                    case Key.S:
                        KeysDown[Key.S] = true;
                        break;
                    case Key.D:
                        KeysDown[Key.D] = true;
                        break;
                    case Key.Escape:
                        GameWindow.Exit();
                        break;
                }
            }
            void KeyUp(object sender, KeyboardKeyEventArgs e)
            {
                switch (e.Key)
                {
                    case Key.W:
                        KeysDown[Key.W] = false;
                        break;
                    case Key.A:
                        KeysDown[Key.A] = false;
                        break;
                    case Key.S:
                        KeysDown[Key.S] = false;
                        break;
                    case Key.D:
                        KeysDown[Key.D] = false;
                        break;
                }
            }
            void MouseWheel(object sender, MouseWheelEventArgs e) //triggered whenever the mouse wheel moves
            {
                Camera.Move(0, 0, -e.Delta / Camera.MoveSensitivity);
            }

            //assign OnUpdateFrame
            DebugWriteLine("Adding update frame events");
            GameWindow.UpdateFrame += OnUpdateFrameTimer;
            GameWindow.UpdateFrame += ObjectUpdateFrame;
            GameWindow.UpdateFrame += CameraUpdateFrame;

            //assign input events
            DebugWriteLine("Adding input events");
            GameWindow.MouseDown += MouseDown;
            GameWindow.MouseUp += MouseUp;
            GameWindow.MouseMove += MouseMove;
            GameWindow.KeyDown += KeyDown;
            GameWindow.KeyUp += KeyUp;
            GameWindow.MouseWheel += MouseWheel;


            //Exit
            while (GameWindow.Exists) { } //wait until the window is exited
            ConsoleApp.ShowConsole();
            DebugWriteLine("Shutting down");

            //end of thread
        }

    }

}
