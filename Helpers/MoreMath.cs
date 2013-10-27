using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bEngine.Helpers
{
    public static class MoreMath
    {
        public static double RadToDeg(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static double DegToRad(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
