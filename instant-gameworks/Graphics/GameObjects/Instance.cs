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
