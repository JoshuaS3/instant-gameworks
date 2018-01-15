using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace InstantGameworks.Graphics
{
    class ObjectFactory
    {

        public static RenderObject CreateSolidCube(float side, Color4 color)
        {
            side = side / 2f;
            Vertex[] vertices =
            {
                new Vertex(new Vector4(-side, -side, -side, 1.0f),   color),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),

                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),

                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),

                new Vertex(new Vector4(side, -side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),
                new Vertex(new Vector4(side, side, side, 1.0f),      color),

                new Vertex(new Vector4(-side, -side, -side, 1.0f),   color),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),

                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),

                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),

                new Vertex(new Vector4(side, side, -side, 1.0f),     color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, side, 1.0f),      color),

                new Vertex(new Vector4(-side, -side, -side, 1.0f),   color),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),

                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),

                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),

                new Vertex(new Vector4(-side, side, side, 1.0f),     color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, side, 1.0f),      color),
            };
            return new RenderObject(vertices);
        }

    }
}
