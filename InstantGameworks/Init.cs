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

            Console.WriteLine("Instant Gameworks (c)2018");
            DebugWriteLine("Init");


            //Main
            
            Console.Title = "Instant Gameworks";

            DebugWriteLine("Window init");
            Graphics.GameworksWindow game = new Graphics.GameworksWindow();
            DebugWriteLine("Window success");
            game.Run(144,144);
            


            
            DebugWriteLine("Shutting down");


        }

    }

}
