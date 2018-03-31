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
