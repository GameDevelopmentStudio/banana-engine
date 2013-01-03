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
    class bMaskList : bMask
    {
        public bMask[] masks;

        public bMaskList(bMask[] masks, int x, int y)
            : base(0, 0, 0, 0)
        {
            // compute dimensions
            if (masks.Length == 0)
            {
                Console.WriteLine("Could not init bMaskList, not one mask was provided");
                return;
            }
            this.rect = new Rectangle(0, 0, 0, 0);
            this.offsetx = int.MaxValue;
            this.offsety = int.MaxValue;
            this.masks = masks;

            // compute offsets
            foreach (var mask in masks)
            {
                offsetx = Math.Min(this.offsetx, mask.rect.X);
                offsety = Math.Min(this.offsety, mask.rect.Y);
                mask.x = x;
                mask.x = y;
            }

            // compute dimensions (need minimum offsets for this)
            foreach (var mask in masks)
            {
                // believe me on this one
                if ((this.rect.Width + this.offsetx) < mask.rect.Width + mask.offsetx)
                    this.rect.Width = mask.rect.Width + mask.offsetx - this.offsetx;
                if ((this.rect.Height + this.offsety) < (mask.rect.Height + mask.offsety))
                    this.rect.Height = mask.rect.Height + mask.offsety - this.offsety;
            }

            this.x = x;
            this.y = y;
        }

        public static new bMask MaskFromFile(StreamReader sr, string src = "", int id = -1)
        {
            string line = sr.ReadLine();

            int msize = 0;
            bMask[] masks = null;

            // get bMask list size
            if (line != null)
            {
                // mask initialization
                try
                {
                    msize = Convert.ToInt32(line.Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries)[0]);
                    masks = new bMask[msize];
                }
                catch (Exception e)
                {
                    if (e is FormatException || e is OverflowException || e is IndexOutOfRangeException)
                    {
                        Console.WriteLine("Could not read masks from file " + src + ", size attribute has errors: " + e.Message);
                        return null;
                    }
                    else
                        // not our division
                        throw;
                }
            }

            int nmasks = 0;
            while (nmasks < msize && line != null)
            {
                bMask mask = bMask.MaskFromFile(sr, src, nmasks);
                if (mask != null)
                    masks[nmasks] = mask;
                else
                    return null;

                nmasks++;
            }

            bMaskList maskList = new bMaskList(masks, 0, 0);
            return maskList;
        }

        public override bool collides(bMask other)
        {
            bool collided = false;

            // collides with outer mask?
            if (other is bSolidGrid)
                collided = other.collides(this);
            else
                collided = rect.Intersects(other.rect);

            // collides with any inner mask?
            if (collided)
                foreach (bMask mask in masks)
                {
                    // lazy (x,y) update
                    mask.x = rect.X - offsetx;
                    mask.y = rect.Y - offsety;
                    if (mask.collides(other))
                        return true;
                }
            return false;
        }

        public override void render(SpriteBatch sb, bGame game, Color color)
        {
            base.render(sb, game, color);

            foreach (bMask mask in masks)
            {
                // lazy (x,y) update
                mask.x = rect.X - offsetx;
                mask.y = rect.Y - offsety;
                mask.render(sb, game, color);
            }
        }
    }
}
