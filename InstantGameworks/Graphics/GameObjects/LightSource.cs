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

        public virtual Color4 DiffuseColor { get; set; }
        public virtual Color4 SpecularColor { get; set; }
        public virtual Color4 AmbientColor { get; set; }
        public virtual Color4 EmitColor { get; set; }
        public virtual float Intensity { get; set; }
        public virtual bool Enabled { get; set; }

        public LightSource()
        {
            DiffuseColor = new Color4(0, 0, 0, 255);
            SpecularColor = new Color4(255, 255, 255, 0);
            AmbientColor = new Color4(0, 0, 0, 0);
            EmitColor = new Color4(0, 0, 0, 0);
            Intensity = 64;
            Enabled = true;
        }
    }

    public class DirectionalLight : LightSource
    {
        public override string ClassName { get; } = "DirectionalLight";
        public override string Name { get; set; } = "DirectionalLight";

        public override Color4 DiffuseColor { get; set; }
        public override Color4 SpecularColor { get; set; }
        public override Color4 AmbientColor { get; set; }
        public override Color4 EmitColor { get; set; }
        public override float Intensity { get; set; }
        public override bool Enabled { get; set; }

        private Vector3 _relativeDirection;
        public Vector3 RelativeDirection {
            get { return _relativeDirection; }
            set { value.NormalizeFast(); _relativeDirection = value; } // normalize the direction
        }

        public DirectionalLight() : base()
        {
            RelativeDirection = new Vector3(0, -1, 0);
        }
    }

    public class PointLight : LightSource
    {
        public override string ClassName { get; } = "PointLight";
        public override string Name { get; set; } = "PointLight";

        public override Color4 DiffuseColor { get; set; }
        public override Color4 SpecularColor { get; set; }
        public override Color4 AmbientColor { get; set; }
        public override Color4 EmitColor { get; set; }
        public override float Intensity { get; set; }
        public override bool Enabled { get; set; }
        
        public Vector3 Position { get; set; }
        public float Radius { get; set; }

        public PointLight() : base()
        {
            Radius = 8;
            Position = new Vector3(0, 0, 0);
        }
    }
}
