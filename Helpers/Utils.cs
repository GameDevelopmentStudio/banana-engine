using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace bEngine.Helpers
{
    public static class Utils
    {
        public static Vector2 add(Vector2 v, Point p)
        {
            return new Vector2(v.X + p.X, v.Y + p.Y);
        }

        public static Vector2 subtract(Vector2 v, Point p)
        {
            return new Vector2(v.X - p.X, v.Y - p.Y);
        }

        public static Color stringToColor(string color)
        {
            int r = 255, g = 0, b = 255;
            // Web color (#RRGGBB)
            if (color[0] == '#' && color.Length == 7)
            {
                r = hexToInt(color.Substring(1, 2));
                g = hexToInt(color.Substring(3, 2));
                b = hexToInt(color.Substring(5, 2));
            }

            return new Color(r, g, b);
        }

        public static int hexToInt(string hex)
        {
            List<char> chars = new List<char>{'0', '1', '2', '3', '4', 
                            '5', '6', '7', '8', '9', 
                            'A', 'B', 'C', 'D', 'E', 'F'};
            int value = 0;
            int count = 0;
            for (int index = hex.Length-1; index >= 0; index--)
            {
                char d = hex[index];
                value += (chars.IndexOf(d)) * (int) Math.Pow(16, count++);
            }

            return value;
        }
    }
}
