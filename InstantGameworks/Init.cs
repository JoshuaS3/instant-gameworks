using System;
using System.Runtime.InteropServices;
using System.Threading;

using Global.Services;
using System.Windows.Forms;

namespace Global
{

    class Init
    {

        private static void DebugWriteLine(string output) => Console.WriteLine(SysTime.GetTime() + " " + output);

        [STAThread]
        public static void Main()
        {

            Console.WriteLine("Instant Gameworks (c)2018");
            DebugWriteLine("Init");


            //Main



            DebugWriteLine("Running");
            DebugWriteLine("Shutting down");


        }

    }

}
