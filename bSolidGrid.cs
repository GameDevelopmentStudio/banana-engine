using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bEngine.Graphics;

namespace bEngine
{
    public class bSolidGrid : bMask
    {
        public int columns, rows;
        public int tileWidth, tileHeight;
        public bool[,] solidData;

        public bSolidGrid(int cols, int rows, int tileWidth, int tileHeight) : base(0, 0, cols * tileWidth, rows * tileHeight)
        {
            columns = cols;
            this.rows = rows;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            solidData = new bool[columns, rows];
            solidData.Initialize();
        }

        public bool parseSolids(string[] src)
        {
            if (src.Length != rows)
                return false;

            bool[,] tempData = new bool[columns, rows];

            int yy = 0;
            foreach (string r in src)
            {
                if (r.Length != columns)
                    return false;
                for (int xx = 0; xx < columns; xx++)
                {
                    tempData[xx, yy] = (r[xx] == '1');
                }
                yy++;
            }

            solidData = tempData;

            return true;
        }

        public bool rectMaskCollides(bMask other)
        {
            int tlx = Math.Max((other.x - x) / tileWidth, 0);
            int tly = Math.Max((other.y - y) / tileHeight, 0);
            int brx = Math.Min((other.x + other.w - 1 - x) / tileWidth, columns - 1);
            int bry = Math.Min((other.y + other.h - 1 - y) / tileHeight, rows - 1);

            for (int xx = tlx; xx <= brx; xx++)
                for (int yy = tly; yy <= bry; yy++)
                    if (solidData[xx, yy])
                        return true;

            return false;
        }

        public override bool collides(bMask other)
        {
            if (!(other is bMaskList))  // this call is useless for masklists, as it will delegate back on this class
            {
                // Bbox check
                if (!base.collides(other))
                    return false;
            }
            
            return rectMaskCollides(other);
        }

        public override void render(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (bConfig.DEBUG)
            {
                Rectangle r = new Rectangle(0, 0, tileWidth, tileHeight);
                for (int xx = 0; xx < columns; xx++)
                    for (int yy = 0; yy < rows; yy++)
                    {
                        if (solidData[xx, yy])
                        {
                            r.X = x + xx * tileWidth;
                            r.Y = y + yy * tileHeight;

                            sb.Draw(bDummyRect.sharedDummyRect(game), r, Color.PowderBlue);
                        }
                    }
            }
        }
    }
}
