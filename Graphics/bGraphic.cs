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
        int _width, _height;
        public virtual int width { get { return _width; } set { _width = value; } }
        public virtual int height { get { return _height; } set { _height = value; } }
        public Color color = Color.White;
        public float alpha 
        { 
            get { return color.A / 256.0f; } 
            set { color.A = (byte) (value*256); } 
        }

        public int offsetx = 0, offsety = 0;

        virtual public void render(SpriteBatch sb, int x, int y)
        {
            render(sb, new Vector2(x, y));
        }

        virtual public void render(SpriteBatch sb, Vector2 position)
        {
        }
    }
}
