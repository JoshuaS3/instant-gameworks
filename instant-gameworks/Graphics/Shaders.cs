using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using OpenTK.Graphics.OpenGL4;


namespace InstantGameworks.Graphics
{
    class Shaders
    {

        public static int CreateShader(string ShaderPath, ShaderType ShaderType)
        {
            int newShader = GL.CreateShader(ShaderType);
            GL.ShaderSource(newShader, File.ReadAllText(@ShaderPath));
            GL.CompileShader(newShader);

            return newShader;
        }

        public static void CompileShaders(int Program, List<int> Shaders)
        {
            foreach (int Shader in Shaders)
            {
                GL.AttachShader(Program, Shader);
            }
            GL.LinkProgram(Program);
            foreach (int Shader in Shaders)
            {
                GL.DetachShader(Program, Shader);
            }
        }

        public static void DeleteShader(int Shader)
        {
            GL.DeleteShader(Shader);
        }

    }
}
