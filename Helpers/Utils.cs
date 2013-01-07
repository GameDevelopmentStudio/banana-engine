using System;

using Microsoft.Xna.Framework;

namespace BananaEngine.Helpers
{
    public static class Utils
    {
        public static Vector2 add(Vector2 v, Point p)
        {
            return new Vector2(v.X + p.X, v.Y + p.Y);
        }
    }
}
