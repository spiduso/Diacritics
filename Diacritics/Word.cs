using System;
using System.Collections.Generic;
using System.Text;

namespace Diacritics
{
    class Word
    {
        public string asciiword;
        public List<Variation> variations;

        public Word(string word, string prev, string next)
        {
            variations = new List<Variation>();
            asciiword = RemoveDiacritics(word);
            Variation helper = new Variation(word, prev, next);
            variations.Add(helper);
        }

        public Word(string word)
        {
            this.asciiword = RemoveDiacritics(word);
            variations = new List<Variation>();
        }

        public Word()
        {

        }

        public void AddVariation(Word whelper, string word, string prev, string next)
        {
            Variation vhelper = new Variation(word, prev, next);
            whelper.variations.Add(vhelper);
        }

        public static string RemoveDiacritics(string word)
        {
            string charsFrom = "áčďéěíňóřšťúůýžÁČĎÉĚÍŇÓŘŠŤÚŮÝŽ";
            string charsTo = "acdeeinorstuuyzACDEEINORSTUUYZ";

            for (int i = 0; i < word.Length; i++)
            {
                for (int j = 0; j < charsFrom.Length; j++)
                {
                    if (word[i] == charsFrom[j])
                    {
                        word = word.Replace(word[i], charsTo[j]);
                        j = charsFrom.Length;
                    }
                }

            }
            return word.ToLower();
        }
    }
}
