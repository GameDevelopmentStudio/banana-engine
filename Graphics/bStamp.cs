using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEngine.Graphics
{
    public class bStamp : bGraphic
    {
        public bool flipped;
        protected Texture2D image;
        public Rectangle source;

        public bStamp(Texture2D image, Rectangle source) : this(image)
        {
            source.X = Math.Min(Math.Max(source.X, 0), image.Width);
            source.Y = Math.Min(Math.Max(source.Y, 0), image.Height);
            source.Width = Math.Min(Math.Max(source.Width, 0), image.Width);
            source.Height = Math.Min(Math.Max(source.Height, 0), image.Height);

            this.source = source;

            width = source.Width;
            height = source.Height;
        }

        public bStamp(Texture2D image)
        {
            this.image = image;
            flipped = false;

            source = new Rectangle(0, 0, image.Width, image.Height);
            width = source.Width;
            height = source.Height;
        }

        override public void render(SpriteBatch sb, Vector2 position)
        {
            Rectangle dest = new Rectangle((int) position.X, (int) position.Y, source.Width, source.Height);
            if (flipped)
                sb.Draw(image, dest, source, color, 0, Vector2.Zero, (flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);
            else
                sb.Draw(image, position, source, color);
        }
    }
}
