using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bEngine.Graphics;
using bEngine.Helpers;

namespace bEngine
{
    public class bMask
    {
        public Rectangle rect;
        public int offsetx, offsety;
        public Texture2D gfx;

        public int x
        {
            get { return rect.X; }
            set { rect.X = value + offsetx; }
        }
        
        public int y
        {
            get { return rect.Y; }
            set { rect.Y = value + offsety; }
        }

        public int w
        {
            get { return rect.Width; }
            set { rect.Width = value; }
        }

        public int h
        {
            get { return rect.Height; }
            set { rect.Height = value; }
        }

        protected bGame _game;
        public bGame game 
        { 
            get { return _game; }
            set { _game = value; gfx = bDummyRect.sharedDummyRect(_game); } 
        }

        public bMask(int x, int y, int w, int h, int offsetx = 0, int offsety = 0)
        {
            this.offsetx = offsetx;
            this.offsety = offsety;
            this.rect = new Rectangle(x + offsetx, y + offsety, w, h);
        }

        public static bMask MaskFromFile(StreamReader sr, string src="", int id=-1)
        {
            string[] items = sr.ReadLine().Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (items.Length < 1)
            {
                Console.WriteLine("Could not read masks from file " + src + ", mask no" + id + " has no type");
                return null;
            }

            short type = (short)Convert.ToInt32(items[0]);

            // normal mask type
            if (type == 0)
            {
                items = sr.ReadLine().Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length < 4)
                {
                    Console.WriteLine("Could not read masks from file " + src + ", mask no" + id + " has not enough params");
                    return null;
                }
                else
                {
                    try
                    {
                        int offsetx = Convert.ToInt32(items[0]);
                        int offsety = Convert.ToInt32(items[1]);
                        int width = Convert.ToInt32(items[2]);
                        int height = Convert.ToInt32(items[3]);

                        return new bMask(0, 0, width, height, offsetx, offsety);
                    }
                    catch (Exception e)
                    {
                        if (e is FormatException || e is OverflowException)
                        {
                            Console.WriteLine("Could not read masks from file " + src + ", mask no" + id + "'s attribute has errors: " + e.Message);
                            return null;
                        }
                        else
                            // not our fault
                            throw;
                    }
                }
            }
            // maskList type
            else if (type == 1)
            {
                return bMaskList.MaskFromFile(sr, src, id);
            }

            return null;
        }

        virtual public void update(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        virtual public bool collides(bMask other)
        {
            if (other is bSolidGrid)
                return other.collides(this);
            else
                return rect.Intersects(other.rect);
        }

        virtual public void render(SpriteBatch sb)
        {
            sb.Draw(gfx, rect, rect, Color.Purple);
        }

        virtual public void render(SpriteBatch sb, bGame game, Color color)
        {
            Texture2D rect = bDummyRect.sharedDummyRect(game);
            sb.Draw(rect, this.rect, color);
        }

        virtual public bool load(int[] src)
        {
            return true;
        }
    }
}
