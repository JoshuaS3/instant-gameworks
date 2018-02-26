using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

using InstantGameworks;
using InstantGameworks.Graphics;
using InstantGameworks.Graphics.GameObjects;


namespace InstantGameworks
{
    class Init
    {
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
            
            Logging.LogEvent("Main thread startup");
            
            // Set window settings
            DisplayDevice DefaultDisplay = DisplayDevice.Default;
            GameWindowRefreshRate = 0;//DefaultDisplay.RefreshRate;
            GameWindowSize = new Vector2(1280, 720); 
            GameWindowPosition = new Vector2(0, 0);
            GameWindowBorder = WindowBorder.Fixed;
            GameWindowState = WindowState.Normal;

            // Create window
            Logging.LogEvent("Initializing GameworksWindow");

            ThreadStart GameThread = new ThreadStart(CreateGameworksWindow);
            Thread RunGame = new Thread(GameThread);
            RunGame.Start();

            // Wait for window to initialize
            SpinWait.SpinUntil( () => GameWindow != null && GameWindow.Exists );





            //
            //
            //
            // Game logic
            //
            //
            //

            Logging.LogEvent("Importing resources");
            // Initialize camera
            StudioCamera Camera = new StudioCamera
            {
                MoveSensitivity = 0.01f
            };
            GameWindow.Camera = Camera;

            // Establish lighting
            var Sun = GameWindow.AddDirectionalLight();
            Sun.Name = "Sun";
            Sun.DiffuseColor = Color4.Red;
            Sun.RelativeDirection = new Vector3(0, -1, 0);

            // Import objects
            var Land = GameWindow.AddObject(@"Testing\sphere.igwo");
            Land.Color = Color4.Red;
            Land.Position = new Vector3(0, 0, -1);

            double _lastTime = 0;
            double _time = 0;
            void OnUpdateFrameTimer(object sender, FrameEventArgs e)
            {
                _lastTime = _time;
                _time += e.Time;
            }
            void ObjectUpdateFrame(object sender, FrameEventArgs e)
            {
                Land.Rotation += new Vector3(0, 0.01f, 0);
            }

            // Camera implementation
            Dictionary<Key, bool> KeysDown = new Dictionary<Key, bool>() { [Key.W] = false, [Key.A] = false, [Key.S] = false, [Key.D] = false };

            Vector2 LastMousePosition = new Vector2(0, 0);
            bool IsRightMouseDown = false;
            bool IsSettingMousePosition = false;
            void CameraUpdateFrame(object sender, FrameEventArgs e)
            {
                if (IsRightMouseDown)
                {
                    IsSettingMousePosition = true;
                    Mouse.SetPosition(LastMousePosition.X + GameWindow.X + 8, LastMousePosition.Y + GameWindow.Y + 31);
                }

                float AdjustedSpeedForFramerate = 144f / (1f / ((float)_time - (float)_lastTime));
                if (KeysDown[Key.W] == true)
                {
                    Camera.Move(0, 0, -AdjustedSpeedForFramerate);
                }
                if (KeysDown[Key.A] == true)
                {
                    Camera.Move(AdjustedSpeedForFramerate, 0, 0);
                }
                if (KeysDown[Key.S] == true)
                {
                    Camera.Move(0, 0, AdjustedSpeedForFramerate);
                }
                if (KeysDown[Key.D] == true)
                {
                    Camera.Move(-AdjustedSpeedForFramerate, 0, 0);
                }
            }

            
            void MouseDown(object sender, MouseButtonEventArgs e)
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
            void MouseUp(object sender, MouseButtonEventArgs e)
            {
                if (e.Button == MouseButton.Right)
                {
                    IsRightMouseDown = false;
                }
            }
            void MouseMove(object sender, MouseMoveEventArgs e)
            {
                if (GameWindow.Focused && IsRightMouseDown && !IsSettingMousePosition)
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
            void MouseWheel(object sender, MouseWheelEventArgs e)
            {
                Camera.Move(0, 0, -e.Delta / Camera.MoveSensitivity * 0.25f);
            }

            //assign OnUpdateFrame
            Logging.LogEvent("Adding update frame events");
            GameWindow.UpdateFrame += OnUpdateFrameTimer;
            GameWindow.UpdateFrame += ObjectUpdateFrame;
            GameWindow.UpdateFrame += CameraUpdateFrame;

            //assign input events
            Logging.LogEvent("Adding input events");
            GameWindow.MouseDown += MouseDown;
            GameWindow.MouseUp += MouseUp;
            GameWindow.MouseMove += MouseMove;
            GameWindow.KeyDown += KeyDown;
            GameWindow.KeyUp += KeyUp;
            GameWindow.MouseWheel += MouseWheel;


            //Exit
            RunGame.Join();
            NativeMethods.ConsoleApp.ShowConsole();
            Logging.LogEvent("Shutting down");
            
            /*Logging.WriteToFile();
            Logging.DisplayLog().Join();
            *///end of thread
        }

    }

}
