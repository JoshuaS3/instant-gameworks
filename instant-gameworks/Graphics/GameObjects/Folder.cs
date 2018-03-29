using System;
using System.Collections.Generic;
using System.Text;

namespace InstantGameworks.Graphics.GameObjects
{
    public class Folder : Instance
    {

        public override string ClassName { get; } = "Folder";
        public override string Name { get; set; } = "Folder";

        public Folder() { }
        public Folder(string name) { Name = name; }

    }
}
