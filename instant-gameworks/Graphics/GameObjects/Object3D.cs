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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using InstantGameworksObject;

namespace InstantGameworks.Graphics.GameObjects
{
    struct DrawVertex
    {
        public static int SizeInBytes = (4 + 3 + 3) * 4;
        public Vector4 position;
        public Vector3 normal;
        public Vector3 texture;
    }

    public class Object3D : Instance
    {
        public override string ClassName { get; } = "Object3D";
        public override string Name { get; set; } = "Object3D";

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 RotationalVelocity { get; set; }

        public Color4 DiffuseColor { get; set; }
        public Color4 SpecularColor { get; set; }
        public Color4 AmbientColor { get; set; }
        public Color4 EmitColor { get; set; }
        public float EmitIntensity { get; set; }

        public int VertexCount { get => _vertexCount; }
        public bool DoRender { get; set; }



        public delegate void RenderEvent(object sender, EventArgs e);
        public event RenderEvent OnRender = delegate { };

        private int _vertexCount;
        private Position[] _vertexPositions;
        private Position[] _vertexNormals;
        private TextureCoordinates[] _vertexTextureCoordinates;
        private Face[] _faces;

        private DrawVertex[] _sortedVertices;

        private bool _initialized;
        private int _objectArray;

        private int _vertexPositionBuffer;
        private Matrix4 _modelView;

        public Object3D(InstantGameworksObject.InstantGameworksObject data)
        {
            _init(data.Positions, data.TextureCoordinates, data.Normals, data.Faces);
        }

        internal void _init(Position[] _vertPos, TextureCoordinates[] _vertText, Position[] _vertNorm, Face[] _face)
        {
            // Set default internal values
            Position = new Vector3(0, 0, 0);
            Scale = new Vector3(1, 1, 1);
            Rotation = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
            RotationalVelocity = new Vector3(0, 0, 0);
            DiffuseColor = new Color4(200, 200, 200, 255);
            SpecularColor = new Color4(50, 50, 50, 0);
            AmbientColor = new Color4(30, 30, 30, 0);
            EmitColor = new Color4(0, 0, 0, 0);
            _vertexCount = _face.Length * 3;
            DoRender = true;

            _vertexPositions = _vertPos;
            _vertexTextureCoordinates = _vertText;
            _vertexNormals = _vertNorm;
            _faces = _face;

            _sortData();

            _objectArray = GL.GenVertexArray();

            _vertexPositionBuffer = GL.GenBuffer();


            //Position buffer

            _updateVBO();

            _initialized = true;
        }
        private void _updateVBO()
        {
            GL.BindVertexArray(_objectArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBuffer);
            GL.NamedBufferData(_vertexPositionBuffer, _sortedVertices.Length * DrawVertex.SizeInBytes, _sortedVertices, BufferUsageHint.StaticDraw);

            GL.VertexArrayAttribBinding(_objectArray, 0, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 0);
            GL.VertexArrayAttribFormat(_objectArray, 0, 4, VertexAttribType.Float, false, 0);
            
            GL.VertexArrayAttribBinding(_objectArray, 1, 0);
            GL.EnableVertexArrayAttrib(_objectArray, 1);
            GL.VertexArrayAttribFormat(_objectArray, 1, 3, VertexAttribType.Float, false, 16);

            GL.VertexArrayVertexBuffer(_objectArray, 0, _vertexPositionBuffer, IntPtr.Zero, DrawVertex.SizeInBytes); //set _positionBuffer as part of _vertexArray
            
        }

        private void _sortData()
        {
            _sortedVertices = new DrawVertex[_faces.Length * 3];

            int count = 0;
            foreach (Face face in _faces)
            {
                foreach (Vertex vertex in face.Vertices)
                {
                    DrawVertex nv = new DrawVertex();

                    Position thisPos = _vertexPositions[vertex.PositionIndex - 1];
                    nv.position = new Vector4(thisPos.X, thisPos.Y, thisPos.Z, 1);


                    
                    if (vertex.NormalIndex != -1)
                    {
                        Position thisNorm = _vertexNormals[vertex.NormalIndex - 1];
                        nv.normal = new Vector3(thisNorm.X, thisNorm.Y, thisNorm.Z);
                    }
                    else
                    {
                        nv.normal = new Vector3(0, 1, 0);
                    }

                    /*
                    if (vertex.TextureCoordinatesIndex != -1)
                    {
                        TextureCoordinates thisTexture = VertexTextureCoordinates[vertex.TextureCoordinatesIndex - 1];
                        nv.texture = new Vector3(thisTexture.U, thisTexture.V, thisTexture.W);
                    }
                    else
                    {
                        nv.texture = new Vector3(0, 0, 0);
                    }*/


                    _sortedVertices[count] = nv;

                    count++;
                }
            }
        }

        public void Render()
        {
            OnRender(this, EventArgs.Empty);
            _modelView = Matrix4.CreateRotationX(Rotation.X) *
                         Matrix4.CreateRotationY(Rotation.Y) *
                         Matrix4.CreateRotationZ(Rotation.Z);
            GL.UniformMatrix4(4, false, ref _modelView);
            _modelView *= Matrix4.CreateScale(Scale.X, Scale.Y, Scale.Z) *
                          Matrix4.CreateTranslation(Position.X, Position.Y, Position.Z);
            GL.UniformMatrix4(2, false, ref _modelView);

            GL.BindVertexArray(_objectArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_initialized)
                {
                    GL.DeleteVertexArray(_objectArray);
                    GL.DeleteBuffer(_vertexPositionBuffer);
                    _initialized = false;
                }
            }
        }
    }
}
