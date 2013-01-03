using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEngine.Graphics
{
    public class bSpritemap : bGraphic
    {
        public Texture2D image;
        public int spriteWidth, spriteHeight;
        public int columns, rows;

        public bAnim currentAnim;
        public Dictionary<string, bAnim> animations;

        public bool flipped = false;

        int _width, _height;

        public new int width { get { return spriteWidth; } }
        public new int height { get { return spriteHeight; } }

        public bSpritemap(Texture2D image, int spriteWidth, int spriteHeight)
        {
            this.image = image;
            this._width = image.Width;
            this._height = image.Height;

            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;

            columns = _width / spriteWidth;
            rows = _height / spriteHeight;

            animations = new Dictionary<string, bAnim>();
            currentAnim = null;
        }

        virtual public void add(bAnim animation)
        {
            animations.Add(animation.name, animation);
        }

        virtual public void play(string anim)
        {
            if (currentAnim == null || currentAnim.name != anim)
            {
                currentAnim = animations[anim];
                if (currentAnim != null)
                {
                    currentAnim.play();
                }
            }
        }

        virtual public void update()
        {
            if (currentAnim != null)
                currentAnim.update();
        }

        virtual protected Rectangle getFrame(int frame)
        {
            Rectangle rect = new Rectangle((frame % columns) * spriteWidth,
                                           (frame / columns) * spriteHeight,
                                           spriteWidth, spriteHeight);
            return rect;
        }

        override public void render(SpriteBatch sb, Vector2 position)
        {
            Rectangle to = new Rectangle((int) position.X, (int) position.Y, spriteWidth, spriteHeight);
            sb.Draw(image, to, getFrame(currentAnim.frame), color, 0, Vector2.Zero, (flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);
        }
    }

    public class bAnim
    {
        public string name;
        public int[] frames;
        public float speed;
        public bool loop;

        public bool playing;
        public int frame 
        {
            set { currentFrame = value; }
            get { currentFrame = frames[currentFrameIndex]; return currentFrame; }
        }
        protected int currentFrame;
        public bool finished;
        public int frameIndex
        {
            get { return currentFrameIndex; }
            set 
            { 
                currentFrameIndex = Math.Max(0, Math.Min(value, frames.Length-1)); 
                actualFrameIndex = currentFrameIndex; 
            }
        }

        protected int currentFrameIndex;
        protected float actualFrameIndex;

        public bAnim(String name, int[] frames, float speed = 1.0f, bool loop = true)
        {
            this.name = name;
            this.frames = frames;
            this.speed = speed;
            this.loop = loop;

            currentFrameIndex = 0;
            currentFrame = frames[currentFrameIndex];
            finished = false;
            playing = false;
        }

        virtual public void play(int from = 0)
        {
            finished = false;
            playing = true;
            currentFrameIndex = from;
            currentFrame = frames[currentFrameIndex];
            actualFrameIndex = from;
        }

        virtual public void pause()
        {
            playing = false;
        }

        virtual public void resume()
        {
            playing = true;
        }

        virtual public void stop()
        {
            playing = false;
            finished = true;
        }

        virtual public void update()
        {
            if (playing && !finished)
            {
                actualFrameIndex += speed;
                if ((int)Math.Floor(actualFrameIndex) >= currentFrameIndex+1)
                {
                    // Next frame reached
                    currentFrameIndex++;
                    actualFrameIndex = currentFrameIndex;
                    // Finished?
                    if (currentFrameIndex >= frames.Length)
                    {
                        if (loop)
                        {
                            currentFrameIndex = 0;
                            actualFrameIndex = currentFrameIndex;
                        }
                        else
                        {
                            playing = false;
                            finished = true;
                            currentFrameIndex = frames.Length - 1;
                        }
                    }

                    currentFrame = frames[currentFrameIndex];
                }
            }
        }
    }
}
