using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantGameworks.Graphics.GameObjects
{
    public class Instance : IDisposable
    {
        protected static int _programId;
        private static bool init = false;
        private Instance _parent = null;

        private List<Instance> children = new List<Instance>();
        public bool Archivable { get; set; } = true;
        public virtual string ClassName { get; } = "Instance";
        public virtual string Name { get; set; } = "Instance";
        public virtual Instance Parent { get => _parent;
            set { value.children.Add(this); _parent = value; }
        }

        public static void AssignPID(int pid)
        {
            if (!init)
            {
                init = true;
                _programId = pid;
            }
        }

        public Instance this[Instance child]
        {
            get { return children.Find(x => x.Name == child.Name) ?? null; }
        }

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }
    }






    
}
