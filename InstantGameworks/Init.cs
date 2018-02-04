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

        private static void DebugWriteLine(string output) => Console.WriteLine(Services.SysTime.GetTime() + " " + output); //DebugWriteLine just adds a timestamp to the printout
        
        private static GameworksWindow Window; //Our window's direct object
        private static IntPtr WindowHandle; //Win32's ID for the window

        private static Vector2 WindowSize; //x, y
        private static Vector2 WindowPosition; //x, y
        private static WindowBorder WindowBorder; //custom border settings (should it be resizeable? hidden? etc)
        private static WindowState WindowState; //fullscreen, maximized, minimized, default
        private static float RefreshRate; //FPS
        private static DisplayDevice WindowDisplay; //which display (graphics card) to use

        [STAThread]
        private static void OpenWindow() //This is separate from the main thread because Window.Run() at the bottom yields the thread (nothing runs until it's closed)
        {
            DebugWriteLine("Window init");

            Window = new GameworksWindow(); //create new window
            WindowHandle = Window.WindowInfo.Handle; //assign handle for later. not in use rn but it'll come in handy

            Window.Width = (int)WindowSize.X; //width and height are separate values
            Window.Height = (int)WindowSize.Y;
            Window.WindowBorder = WindowBorder;
            Window.WindowState = WindowState;
            Window.X = (int)WindowPosition.X; //position is also 2 separate values
            Window.Y = (int)WindowPosition.Y;
            DebugWriteLine("Window success");

            Window.VSync = VSyncMode.On; //turn on VSync to prevent the refresh rate from going over the display's default
            Window.Run(RefreshRate, RefreshRate); //run the window (begins the render pipeline)
        }

        [STAThread]
        public static void Main() //this is the main thread (runs first)
        {
            Console.Title = "Instant Gameworks";
            Console.WriteLine("Instant Gameworks (c)  2018");

            //Main
            DebugWriteLine("Init");

            //Set window settings
            WindowDisplay = DisplayDevice.Default; //default display
            RefreshRate = 144; //limit FPS to 144hz
            WindowSize = new Vector2(1280, 720); //set window size to 1280x720
            WindowPosition = new Vector2(100, 50); //a bit from the top left corner
            WindowBorder = WindowBorder.Fixed; //can't resize window
            WindowState = WindowState.Normal; //not maximized or minimized

            //Create window
            ThreadStart GameThread = new ThreadStart(OpenWindow);
            Thread RunGame = new Thread(GameThread);
            RunGame.Start(); //create the window as a separate thread (it will run in the background)

            //Wait for window to initialize
            while (Window == null) { }



            //
            //
            //
            //
            //
            //
            //   THIS POINT ON IS ALL GAME LOGIC. EVERYTHING ELSE IS HANDLED INTERNALLY BY THE GAMEWORKSWINDOW CLASS
            //
            //
            //
            //
            //
            //



            //Add game objects
            StudioCamera Camera = new StudioCamera(); //Create new studio-esque camera, ability to move with WASD, right mouse button, and scroll wheel
            Camera.MoveSensitivity = 0.08f; //Set the movement sensitivity
            Window.Camera = Camera; //Assign the current view camera to be this
            
            
            var Airplane = Window.AddObject(@"Testing\airplane.igwo"); //Import "airplane.igwo" directly into the render queue
            Airplane.Scale = new Vector3(10, 10, 10);
            Airplane.Position = new Vector3(0, 0, -4);

            var Land = Window.AddObject(@"Testing\land.igwo"); //Import "land.igwo" directly into the render queue
            Land.Scale = new Vector3(10, 30, 10);
            Land.Position = new Vector3(0, -20, 0);
            
            

            bool[] KeysDown = new bool[] { false, false, false, false }; //used to implement the camera (WASD keys)


            //mouse settings for camera
            Vector2 mouseLastPos = new Vector2(0, 0);
            bool isRightMouseDown = false;
            bool settingMousePosition = false;

            double _time = 0;
            float k;
            void OnUpdateFrame(object sender, FrameEventArgs e)
            {
                _time += e.Time; //add time since last frame


                //give airplane a cartoonish flying animation
                k = (float)_time * 0.3f;
                Airplane.Rotation = new Vector3((float)Math.Cos(_time * 0.75f) * 0.1f,
                                                (float)Math.Sin(_time * 0.5f) * 0.1f + 0.4f,
                                                (float)Math.Sin(_time * 0.5f) * 0.125f);
                Airplane.Position = new Vector3((float)(Math.Sin(k) * 0.01f), (float)(Math.Cos(k * 5f) * 0.025f), -4.0f);


                //set camera's hue
                float hue = ((float)_time * 0.1f) % 1f;
                var color = OpenTK.Graphics.Color4.FromHsv(new Vector4(hue, 0.5f, 0.5f, 0.5f));
                Airplane.Color = color; //we can set color!

                if (isRightMouseDown) //if the camera is being rotated, keep the mouse in the same spot on the screen
                {
                    settingMousePosition = true;
                    Mouse.SetPosition(mouseLastPos.X + Window.X + 8, mouseLastPos.Y + Window.Y + 31);
                }
                
                if (KeysDown[0] == true) //W
                {
                    Camera.Move(0, 0, -1);
                }
                if (KeysDown[1] == true) //A
                {
                    Camera.Move(1, 0, 0);
                }
                if (KeysDown[2] == true) //S
                {
                    Camera.Move(0, 0, 1);
                }
                if (KeysDown[3] == true) //D
                {
                    Camera.Move(-1, 0, 0);
                }
            }

            
            void MouseDown(object sender, MouseButtonEventArgs e) //is triggered whenever a mouse button is pressed
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
            void MouseUp(object sender, MouseButtonEventArgs e) //triggered whenever a mouse button is released
            {
                if (e.Button == MouseButton.Right)
                {
                    isRightMouseDown = false;
                }
            }
            void MouseMove(object sender, MouseMoveEventArgs e) //triggered whenever the mouse is moved
            {
                if (Window.Focused && isRightMouseDown && !settingMousePosition) //if the window's in focus, the RMB is down, and the program isn't doing it
                {
                    Camera.AddRotation(e.XDelta, e.YDelta);
                }
                settingMousePosition = false;
            }
            void KeyDown(object sender, KeyboardKeyEventArgs e) //triggered whenever a key is pressed
            {
                switch (e.Key)
                {
                    case Key.W:
                        KeysDown[0] = true;
                        break;
                    case Key.A:
                        KeysDown[1] = true;
                        break;
                    case Key.S:
                        KeysDown[2] = true;
                        break;
                    case Key.D:
                        KeysDown[3] = true;
                        break;
                }
            }
            void KeyUp(object sender, KeyboardKeyEventArgs e) //triggered whenever a key is released
            {
                switch (e.Key)
                {
                    case Key.W:
                        KeysDown[0] = false;
                        break;
                    case Key.A:
                        KeysDown[1] = false;
                        break;
                    case Key.S:
                        KeysDown[2] = false;
                        break;
                    case Key.D:
                        KeysDown[3] = false;
                        break;
                }
            }
            void MouseWheel(object sender, MouseWheelEventArgs e) //triggered whenever the mouse wheel moves
            {
                Camera.Move(0, 0, -e.Delta / Camera.MoveSensitivity);
            }

            //assign OnUpdateFrame
            DebugWriteLine("Adding update frame events");
            Window.UpdateFrame += OnUpdateFrame;

            //assign input events
            DebugWriteLine("Adding input events");
            Window.MouseDown += MouseDown;
            Window.MouseUp += MouseUp;
            Window.MouseMove += MouseMove;
            Window.KeyDown += KeyDown;
            Window.KeyUp += KeyUp;
            Window.MouseWheel += MouseWheel;


            //Exit
            while (Window.Exists) { } //wait until the window is exited
            DebugWriteLine("Shutting down");

            //end of thread
        }

    }

}
