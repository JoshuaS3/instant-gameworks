using System;
using System.Runtime.InteropServices;
using System.Threading;

using InstantGameworks;
using System.Windows.Forms;

namespace InstantGameworks
{

    class Init
    {

        private static void DebugWriteLine(string output) => Console.WriteLine(Services.SysTime.GetTime() + " " + output);

        [STAThread]
        public static void Main()
        {

            Console.Title = "Instant Gameworks";
            Console.WriteLine("Instant Gameworks (c)2018");


            //Main
            
            DebugWriteLine("Init");
            DebugWriteLine("Window init");
            Graphics.GameworksWindow game = new Graphics.GameworksWindow();
            DebugWriteLine("Window success");
            Services.ConsoleApp.HideConsole();
            game.Run(144,144);



            //Exit

            Services.ConsoleApp.ShowConsole();
            DebugWriteLine("Shutting down");


        }

    }

}
