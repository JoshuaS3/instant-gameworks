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

        public LightSource()
        {
            Color = new Color4(255, 255, 255, 255);
            Intensity = 1;
            Enabled = true;
        }
    }

    public class DirectionalLight : LightSource
    {
        public override string ClassName { get; } = "DirectionalLight";
        public override string Name { get; set; } = "DirectionalLight";
        
        public override Color4 Color { get; set; }
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
        
        public override Color4 Color { get; set; }
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
