using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global.Services
{
    class ConsoleApp
    {
        //Import the functions to hide the console window, settings
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5; //Debugging, if it ever needs to be opened once more

        private static IntPtr ConsoleWindow = GetConsoleWindow();

        public static void ShowConsole()
        {
            ShowWindow(ConsoleWindow, SW_SHOW);
        }
        public static void HideConsole()
        {
            ShowWindow(ConsoleWindow, SW_HIDE);
        }
    }
}
