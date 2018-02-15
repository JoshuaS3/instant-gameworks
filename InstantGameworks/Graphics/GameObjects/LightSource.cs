using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace InstantGameworks.Graphics.GameObjects
{
    public class LightSource : Instance
    {
        public override string ClassName { get; } = "LightSource";
        public override string Name { get; set; } = "LightSource";

        public virtual Color4 Color { get; set; }
        public virtual float Intensity { get; set; }
        public virtual bool Enabled { get; set; }
    }

    public class PointLight : LightSource
    {
        public override string ClassName { get; } = "PointLight";
        public override string Name { get; set; } = "PointLight";

        public override Color4 Color { get; set; }
        public override float Intensity { get; set; }
        public override bool Enabled { get; set; }

        public float Radius { get; set; }

        public PointLight(Color4 color, float intensity, float radius)
        {
            Color = color;
            Intensity = intensity;
            Radius = radius;
            Enabled = true;
        }

    }
}
