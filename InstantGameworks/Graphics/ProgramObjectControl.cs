using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace InstantGameworks.Graphics
{
    class ProgramObjectControl
    {

        public static int CreateProgram()
        {
            return GL.CreateProgram();
        }

        public static void DeleteProgram(int ProgramID)
        {
            GL.DeleteProgram(ProgramID);
        }

    }
}
