using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bEngine.Helpers;

namespace bEngine.Graphics
{
    public class bBodyPart
    {
        // render ordering
        public int zindex = 0;

        // translation transform
        public int xoffset;
        public int yoffset;

        // actual pos fro every frame
        public Point[] pos;

        // attached body
        public bBody bodyPart;
    };

    public class bBody : bSpritemap
    {
        protected bGame game;
        protected Color debugColor;

        public Dictionary<string, bBodyPart> attached;
        public string name;

        public bMask[] masks;
        public bMask mask
        {
            set { currentMask = value; }
            get 
            { 
                currentMask = masks[currentAnim.frame];
                return currentMask;
            }

        }
        protected bMask currentMask;

        public Point[] hotspots;
        public Point hotspot
        {
            set { currentHotspot = value; }
            get
            {
                currentHotspot = hotspots[currentAnim.frame];
                return currentHotspot;
            }

        }
        protected Point currentHotspot;

        public bBody(bGame game, string imageSrc, int spriteWidth, int spriteHeight)
            : base(game.Content.Load<Texture2D>(imageSrc), spriteWidth, spriteHeight)
        {
            this.game = game;
            
            // init debug rectangle color
            debugColor = Color.FromNonPremultiplied(Constants.bRandom.Next(0, 255), Constants.bRandom.Next(0, 255), Constants.bRandom.Next(0, 255), 150);

            attached = new Dictionary<string, bBodyPart>();

            // default name
            name = "";

            string path = "Assets" + "\\" + imageSrc + ".cfg";
            parseMasks(path);
        }

        public bool parseMasks(string src)
        {
            using (var sr = System.IO.File.OpenText(src))
            {
                string line = sr.ReadLine();

                // get bMask list size
                if (line != null)
                {
                    int nmasks = 0;
                    int msize = 0;

                    // mask initialization
                    try
                    {
                        msize = Convert.ToInt32(line.Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries)[0]);
                        masks = new bMask[msize];
                        hotspots = new Point[msize];
                    }
                    catch (Exception e)
                    {
                        if (e is FormatException || e is OverflowException || e is IndexOutOfRangeException)
                        {
                            Console.WriteLine("Could not read masks from file " + src + ", size attribute has errors: " + e.Message);
                            return false;
                        }
                        else
                            // not our division
                            throw;
                    }

                    int apsize = 0;
                    int naps = 0;
                    // attach points initialization
                    try
                    {
                        if ((line = sr.ReadLine()) == null) return false;
                        apsize = Convert.ToInt32(line.Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries)[0]);

                        while (naps < apsize && (line = sr.ReadLine()) != null)
                        {
                            string id = line.Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries)[0];
                            bBodyPart bodyPart = new bBodyPart();
                            bodyPart.bodyPart = null;  // body is initially non-existent
                            bodyPart.pos = new Point[msize];  // as many positions as frames
                            attached.Add(id, bodyPart);
                            naps++;
                        }

                        if (naps < apsize)
                        {
                            Console.WriteLine("Could not read masks from file " + src + ", some attach points are missing");
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is FormatException || e is OverflowException || e is IndexOutOfRangeException)
                        {
                            Console.WriteLine("Could not read masks from file " + src + ", attach point attribute has errors: " + e.Message);
                            return false;
                        }
                        else
                            // not our division
                            throw;
                    }                    

                    // fill bMask list
                    while (nmasks < msize)
                    {
                        // Read frame hotspot
                        if ((line = sr.ReadLine()) == null) return false;
                        try
                        {
                            string[] items = line.Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries);
                            hotspots[nmasks] = new Point(Convert.ToInt32(items[0]), Convert.ToInt32(items[1]));
                        }
                        catch (Exception e)
                        {
                            if (e is FormatException || e is OverflowException || e is IndexOutOfRangeException)
                            {
                                Console.WriteLine("Could not read masks from file " + src + ", hotspot attribute has errors: " + e.Message);
                                return false;
                            }
                            else
                                // not our division
                                throw;
                        }

                        // Read frame mask
                        bMask mask = bMask.MaskFromFile(sr, src, nmasks);
                        if (mask != null)
                            masks[nmasks] = mask;
                        else
                            return false;

                        parseAttachPoints(sr, nmasks, apsize, src);
                        nmasks++;
                    }

                    return true;
                }
                else
                    return false;
            }
        }

        public bool parseAttachPoints(StreamReader sr, int frame, int apsize, string fname="")
        {
            int naps = 0;
            string line;
            while (naps < apsize && (line = sr.ReadLine()) != null)
            {
                try
                {
                    string[] items = line.Split(Constants.bCharSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string id = items[0];
                    attached[id].pos[frame] = new Point(Convert.ToInt32(items[1]), Convert.ToInt32(items[2]));

                    naps++;
                }
                catch (Exception e)
                {
                    if (e is FormatException || e is OverflowException || e is IndexOutOfRangeException)
                    {
                        Console.WriteLine("Could not read masks from file " + fname + ", attach point attribute has errors: " + e.Message);
                        return false;
                    }
                    else
                        // not our division
                        throw;
                }
            }

            return true;
        }

        public void attach(string id, bBody body, int xoffset=0, int yoffset=0)
        {
            if (attached.ContainsKey(id))
            {
                bBodyPart bodyPart = attached[id];
                bodyPart.xoffset = xoffset;
                bodyPart.yoffset = yoffset;
                bodyPart.bodyPart = body;
            }
            else
            {
                Console.WriteLine("Couldn't attach " + id + " to body, config file did not include it");
            }
        }

        public Pair<bBody, bBody>[] collides(bBody other)
        {
            List<Pair<bBody, bBody>> collisionPairs = new List<Pair<bBody, bBody>>();

            // compute lists of both bodies
            bBody[] selfBodies = toArray();
            bBody[] otherBodies = other.toArray();
        
            foreach (bBody selfBody in selfBodies)
                foreach (bBody otherBody in otherBodies)
                {
                    if (selfBody.mask.collides(otherBody.mask))
                        collisionPairs.Add(new Pair<bBody, bBody>(selfBody, otherBody));
                }

            return collisionPairs.ToArray();
        }

        public bBody[] toArray()
        {
            List<bBody> bodies = new List<bBody>();

            // Visit root
            bodies.Add(this);
            foreach (bBodyPart bodyPart in attached.Values)
            {
                // Visit
                bodies.Add(bodyPart.bodyPart);

                // Continue traversing
                bodies.Concat(bodies.ToArray());
            }

            return bodies.ToArray();
        }


        public void updateFlippableMask(int x, int y)
        {
            int mx = x;
            if (flipped)
            {
                mx += spriteWidth - 2 * mask.offsetx - mask.w;
            }

            if (mask.GetType() == typeof(bMaskList))
            {
                // lazy update for mask lists (so that they can flip their inner masks)
                ((bMaskList)mask).flipped = flipped;
            }

            mask.update(mx, y);
        }

        public void update(int x, int y)
        {
            base.update();

            // Change mask offset if flipped
            updateFlippableMask(x, y);
            
            foreach (bBodyPart bodyPart in attached.Values)
            {
                if (bodyPart.bodyPart != null)
                {
                    // lazy update of common params
                    bodyPart.bodyPart.flipped = flipped;

                    // inner offsets are different in flipped (similar than with masks, but different)
                    int bodyX = (int) bodyPartxoffset(bodyPart, (float) x);
                    int bodyY = y + bodyPart.pos[currentAnim.frame].Y + bodyPart.yoffset - bodyPart.bodyPart.hotspot.Y;

                    bodyPart.bodyPart.update(bodyX, bodyY);
                }
            }
        }

        public override void render(SpriteBatch sb, Vector2 position)
        {
            base.render(sb, position);
            // if debug, paint rectangle above
            if (bConfig.DEBUG)
            {
                mask.render(sb, game, debugColor);
            }

            // SORT BODIES BY ZINDEX

            foreach (bBodyPart bodyPart in attached.Values.OrderBy(x => x.zindex))
            {
                if (bodyPart.bodyPart != null)
                {
                    // lazy update of common params
                    bodyPart.bodyPart.flipped = flipped;

                    Vector2 tmpPos = new Vector2();
                    tmpPos.X = bodyPartxoffset(bodyPart, position.X);
                    tmpPos.Y = position.Y + bodyPart.pos[currentAnim.frame].Y + bodyPart.yoffset - bodyPart.bodyPart.hotspot.Y;

                    bodyPart.bodyPart.render(sb, tmpPos);
                }
            }
        }

        float bodyPartxoffset(bBodyPart bodyPart, float x)
        {
            // gets natural or mirrored offset
            if (!bodyPart.bodyPart.flipped)
            {
                return x + bodyPart.pos[currentAnim.frame].X + bodyPart.xoffset - bodyPart.bodyPart.hotspot.X;
            }
            else
            {
                return x + spriteWidth - bodyPart.pos[currentAnim.frame].X - bodyPart.xoffset + bodyPart.bodyPart.hotspot.X - bodyPart.bodyPart.spriteWidth;
            }
        }
    }
}
