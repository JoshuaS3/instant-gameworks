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
using System.Text;
using InstantGameworks.Graphics.GameObjects;

namespace InstantGameworks.Graphics
{
    public static class Services
    {
        private class ObjectStorage : Folder
        {
            bool RunScripts = false;

            public ObjectStorage() { }
            public ObjectStorage(string name) { Name = name; }
        }

        public static Folder CreateGameWorld()
        {
            Folder Game = new Folder("Game");
            Folder RenderObjects = new Folder("RenderObjects");
            RenderObjects.Parent = Game;
            Folder GraphicalUserInterface = new Folder("GraphicalUserInterface");
            GraphicalUserInterface.Parent = Game;
            ObjectStorage ObjectStorage = new ObjectStorage("ObjectStorage");
            ObjectStorage.Parent = Game;
            GameObjects.Graphics Graphics = new GameObjects.Graphics("Graphics");

            return Game;
        }

    }
}
