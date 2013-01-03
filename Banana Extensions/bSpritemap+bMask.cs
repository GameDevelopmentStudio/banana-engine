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
        // translation transform
        public int xoffset;
        public int yoffset;

        // actual pos fro every frame
        public Pair<int, int>[] pos;

        // attached body
        public bBody bodyPart;
    };

    public class bBody : bSpritemap
    {
        protected bGame game;
        protected Color debugColor;

        public Dictionary<string, bBodyPart> attached; 

        public bMask[] masks;
        public bMask mask
        {
            set { currentMask = value; }
            get { currentMask = masks[currentAnim.frame]; return currentMask; }

        }
        protected bMask currentMask;

        public bBody(bGame game, string imageSrc, int spriteWidth, int spriteHeight)
            : base(game.Content.Load<Texture2D>(imageSrc), spriteWidth, spriteHeight)
        {
            this.game = game;
            
            // init debug rectangle color
            debugColor = Color.FromNonPremultiplied(Constants.bRandom.Next(0, 255), Constants.bRandom.Next(0, 255), Constants.bRandom.Next(0, 255), 150);

            attached = new Dictionary<string, bBodyPart>();

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
                            bodyPart.pos = new Pair<int, int>[msize];  // as many positions as frames
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
                    while (nmasks < msize && line != null)
                    {
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
                    attached[id].pos[frame] = new Pair<int, int>(Convert.ToInt32(items[1]), Convert.ToInt32(items[2]));

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

        // TODO: collides method(s)

        public void update(int x, int y)
        {
            base.update();

            mask.update(x, y);
            foreach (bBodyPart bodyPart in attached.Values)
            {
                if (bodyPart.bodyPart != null)
                {
                    int bodyX = x + bodyPart.pos[currentAnim.frameIndex].first + bodyPart.xoffset;
                    int bodyY = y + bodyPart.pos[currentAnim.frameIndex].second + bodyPart.yoffset;
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

            foreach (bBodyPart bodyPart in attached.Values)
            {
                if (bodyPart.bodyPart != null)
                {
                    Vector2 tmpPos = new Vector2();
                    tmpPos.X = position.X + bodyPart.pos[currentAnim.frameIndex].first + bodyPart.xoffset;
                    tmpPos.Y = position.Y + bodyPart.pos[currentAnim.frameIndex].second + bodyPart.yoffset;
                    bodyPart.bodyPart.render(sb, tmpPos);
                }
            }
        }
    }
}
