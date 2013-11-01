using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEngine.Graphics
{
    public class bGraphic
    {
        public int width = 0, height = 0;
        public Color color = Color.White;
        public float alpha 
        { 
            get { return color.A / 256.0f; } 
            set { color.A = (byte) (value*256); } 
        }

        virtual public void render(SpriteBatch sb, int x, int y)
        {
            render(sb, new Vector2(x, y));
        }

        virtual public void render(SpriteBatch sb, Vector2 position)
        {
        }
    }
}
