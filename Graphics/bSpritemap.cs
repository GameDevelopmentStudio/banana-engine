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
        public bool vflipped = false;

        int _width, _height;

        public float scaleX, scaleY;

        public override int width { get { return spriteWidth; } }
        public override int height { get { return spriteHeight; } }

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

            scaleX = 1;
            scaleY = 1;
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
            // Avoid crashing if no animation is currently set
            if (currentAnim == null)
                return;
            Rectangle to = new Rectangle((int) position.X + offsetx, (int) position.Y + offsety, (int) (spriteWidth*scaleX), (int) (spriteHeight*scaleY));

            SpriteEffects effects = (flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (vflipped ? SpriteEffects.FlipVertically : SpriteEffects.None);
            sb.Draw(image, to, getFrame(currentAnim.frame), color, 0, Vector2.Zero, effects, 0);
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
        public int firstFrame
        {
            get
            {
                // If speed is positive, flow goes as usual
                if (speed > 0)
                    return 0;
                // If speed is negative, flow is inversed (last frame is first frame and viceversa)
                else
                    return frames.Length - 1;
            }
        }
        public int lastFrame
        {
            get
            {
                // Same logic as firstFrame here
                if (speed > 0)
                    return frames.Length - 1;
                else
                    return  0;
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

            currentFrameIndex = firstFrame;
            currentFrame = frames[currentFrameIndex];
            finished = false;
            playing = false;
        }

        virtual public void play(int from)
        {
            finished = false;
            playing = true;
            currentFrameIndex = from;
            currentFrame = frames[currentFrameIndex];
            actualFrameIndex = from;
        }

        virtual public void play()
        {
            play(firstFrame);
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
                int firstFrame = this.firstFrame;
                int lastFrame = this.lastFrame;

                // if current frame is different than the one on the last step, we have advanced in the animation
                if ( (int)Math.Floor(actualFrameIndex) != currentFrameIndex)
                {
                    // Next frame reached (moving forward or backwards depending on the speed sign)
                    int direction = Math.Sign(speed);
                    currentFrameIndex += direction * 1;
                    actualFrameIndex = currentFrameIndex;
                    // Finished?
                    // If flow is going forward, we are finished if currentFrame > lastFrame, currentFrame - lastFrame > 0
                    // If flow is going backwards, we are finished if currentFrame < lastFrame, -1*(currentFrame - lastFrame) > 0
                    if (direction * (currentFrameIndex - lastFrame) > 0)
                    {
                        if (loop)
                        {
                            currentFrameIndex = firstFrame;
                            actualFrameIndex = currentFrameIndex;
                        }
                        else
                        {
                            playing = false;
                            finished = true;
                            currentFrameIndex = lastFrame;
                        }
                    }

                    currentFrame = frames[currentFrameIndex];
                }
            }
        }
    }
}
