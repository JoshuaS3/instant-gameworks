/*  Copyright (c) Joshua Stockin 2018
 *
 *  This file is part of Instant Gameworks.
 *
 *  Instant Gameworks is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Instant Gameworks is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Instant Gameworks.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantGameworks
{
    public static class NativeMethods
    {
        public static class ConsoleApp
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
}
