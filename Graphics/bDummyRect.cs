using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEngine.Graphics
{
    class bDummyRect
    {
        private static bDummyRect instance;
        private Texture2D texture;

        private bDummyRect(Game game)
        {
           texture = game.Content.Load<Texture2D>("rect");
        }

        public static Texture2D sharedDummyRect(Game game)
        {
            if (instance == null)
                instance = new bDummyRect(game);
            return instance.texture;
        }
    }
}
