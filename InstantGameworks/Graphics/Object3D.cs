using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;

namespace InstantGameworks.Graphics
{
    class Object3D
    {
        public static Vector3 Position { get; set; } = new Vector3(0, 0, 0); //Origin
        public static Vector3 Scale { get; set; } = new Vector3(1, 1, 1); //Default scale
        public static Vector3 Rotation { get; set; } = new Vector3(0, 0, 0); //No rotation
        public static Vector3 Velocity { get; set; } = new Vector3(0, 0, 0); //No movement
        public static Vector3 RotationalVelocity { get; set; } = new Vector3(0, 0, 0); //No movement
        public static Color4 Color { get; set; } = new Color4(0.8f, 0.8f, 0.8f, 1.0f); //Gray
        public static bool DoRender { get; set; } = true; //Renders object if true
        
        private static RenderObject[] Faces;

        public Object3D(RenderObject[] faces)
        {
            Faces = faces;
        }

        private static void RenderObject()
        {

        }
    }
}
