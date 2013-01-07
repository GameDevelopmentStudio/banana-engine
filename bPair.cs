using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaEngine
{
    class Pair<T1, T2>
    {
        T1 first { get; set; }
        T2 second { get; set; }

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }
    }
}
