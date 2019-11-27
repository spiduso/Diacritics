using System;
using System.Collections.Generic;
using System.Text;

namespace Diacritics
{
    class Variation
    {
        public string utfword;
        public int count = 0;
        public List<Pair> pairs;

        public Variation()
        {

        }

        public Variation(string word)
        {
            this.utfword = word;
            pairs = new List<Pair>();
        }

        public Variation(string word, string prev, string next)
        {
            pairs = new List<Pair>();
            utfword = word;
            Pair helper = new Pair(prev, next);
            pairs.Add(helper);
        }

        public bool IsInPairs(string prevS, string nextS)
        {
            if (pairs.Exists(x => x.next.Equals(nextS) && x.prev.Equals(prevS)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddPair(string prevS, string nextS)
        {
            Pair helper = new Pair(prevS, nextS);
            pairs.Add(helper);
        }

        public void ExistInPairs(string prevS, string nextS)
        {
            if (!IsInPairs(prevS, nextS))
                AddPair(prevS, nextS);
        }
    }
}
