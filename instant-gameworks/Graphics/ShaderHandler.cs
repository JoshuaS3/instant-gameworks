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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using OpenTK.Graphics.OpenGL4;


namespace InstantGameworks.Graphics
{

    class ShaderHandler
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

        public static void CompileShader(int Program, int Shader)
        {
            GL.AttachShader(Program, Shader);
            GL.LinkProgram(Program);
            GL.DetachShader(Program, Shader);
        }

        public static void DeleteShader(int Shader)
        {
            GL.DeleteShader(Shader);
        }

    }
}
