using System;
using System.Collections.Generic;
using System.Text;

namespace Diacritics
{
    class Pair
    {
        public string prev;
        public string next;

        public Pair(string prev, string next)
        {
            this.next = next;
            this.prev = prev;
        }

        public Pair()
        {
        }
    }
}
