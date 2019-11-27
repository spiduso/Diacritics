using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Diacritics
{
    class Dictionary
    {
        List<Word>[] arr = new List<Word>[27];
        public List<Word> words = new List<Word>();

        public Dictionary()
        {
            for (int i = 0; i < 27; i++)
            {
                arr[i] = new List<Word>();
            }
        }

        public void AddWordToDict(string word, string prev, string next)
        {
            Word helper = FindWord(word);
            if (helper == null)
            {
                AddToDict(word, prev, next);
            }
            else
            {
                Variation vhelper = FindVariation(helper, word);
                if (vhelper == null)
                {
                    AddToVariations(helper, word, prev, next);
                }
                else
                {
                    if (FindPair(vhelper, prev, next) == null)
                    {
                        AddToPairs(vhelper, prev, next);
                    }
                }
            }
        }

        int ReturnCount(string word)
        {
            Word helper = FindWord(word);

            if (helper != null)
            {
                Variation vhelper = FindVariation(helper, word);
                if (vhelper != null)
                    return vhelper.count;
            }

            return 0;
        }

        int ReturnArrPlace(string word)
        {
            word = Word.RemoveDiacritics(word);
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            int i = 0;
            while (word[0] != alphabet[i] && i < alphabet.Length - 1)
            {
                i++;
            }

            if (i == alphabet.Length - 1 && word[0] != alphabet[i])
                return ++i;
            return i;
        }

        void AddToDict(string word, string prev, string next)
        {
            Word helper = new Word(word, prev, next);
            arr[ReturnArrPlace(word)].Add(helper);
        }

        public Word FindWord(string word)
        {
            int alphabet = ReturnArrPlace(word);
            Word whelper = arr[ReturnArrPlace(word)].Find(x => x.asciiword.Equals(Word.RemoveDiacritics(word)));
            if (whelper != null)
                return whelper;
            return null;
        }

        void AddToVariations(Word helper, string word, string prev, string next)
        {
            Variation vhelper = new Variation(word, prev, next);
            helper.variations.Add(vhelper);
        }

        public Variation FindVariation(Word helper, string word)
        {
            if (helper != null)
            {
                Variation vhelper = helper.variations.Find(x => x.utfword.Equals(word));
                return vhelper;
            }
            return null;
        }

        void AddToPairs(Variation vhelper, string prev, string next)
        {
            vhelper.count++;
            Pair phelper = new Pair(prev, next);
            vhelper.pairs.Add(phelper);
        }

        public Pair FindPair(Variation vhelper, string prev, string next)
        {
            Pair phelper = vhelper.pairs.Find(x => x.prev.Equals(prev) && x.next.Equals(next));

            if (phelper != null)
                return phelper;
            return null;
        }

        void ReadPair(StreamReader sr, ref string prev, ref string next)
        {
            prev = "";
            next = "";
            int c = sr.Read();
            while (c != 124) // '|'
            {
                prev += Convert.ToChar(c);
                c = sr.Read();
            }

            next = sr.ReadLine();
        }

        string ReadVariation(StreamReader sr, int firstchar)
        {
            string word = "";
            word += firstchar;

            int c = sr.Read();
            while (c != 9)
            {
                word += Convert.ToChar(c);
                c = sr.Read();
            }

            return word;
        }

        bool IsItVariation(int first)
        {
            if (first == 9)
                return true;
            return false;
        }

        bool IsItWord(int first)
        {
            if (first != 9)
                return true;
            return false;
        }

        bool IsItPair(int second)
        {
            if (second == 9)
                return true;
            return false;
        }

        string GetASCIIWord(StreamReader sr, int first, int second)
        {
            string word = "";
            string restoftheword = sr.ReadLine();
            word += Convert.ToChar(first);
            word += Convert.ToChar(second);
            word = word + restoftheword;
            return word;
        }

        string GetUTFWord(StreamReader sr, int second)
        {
            string word = "";
            string restoftheword = sr.ReadLine();
            word += Convert.ToChar(second);
            word = word + restoftheword;
            return word;
        }

        void ReadFirstAndSecond(StreamReader sr, ref int first, ref int second)
        {
            if (!sr.EndOfStream)
            {
                first = sr.Read();
                if (!sr.EndOfStream)
                    second = sr.Read();
            }
        }

        public void LoadDict()
        {
            string word = "";
            string next = "";
            string prev = "";

            int pos;
            int first;
            int second;

            Word whelper = new Word();
            Variation vhelper = new Variation();
            Pair phelper = new Pair();
            using (var sr = new StreamReader("dict"))
            {
                first = sr.Read();
                second = sr.Read();

                while (IsItWord(first) && !sr.EndOfStream)
                {
                    if (first != 9)
                    {
                        word = GetASCIIWord(sr, first, second);
                        whelper = new Word(word);

                        ReadFirstAndSecond(sr, ref first, ref second);
                        while (IsItVariation(first) && !sr.EndOfStream)
                        {
                            if (second != 9)
                            {

                                word = GetUTFWord(sr, second);
                                vhelper = new Variation(word);

                                ReadFirstAndSecond(sr, ref first, ref second);

                                while (IsItPair(first) && !sr.EndOfStream)
                                {

                                    ReadPair(sr, ref prev, ref next);
                                    phelper = new Pair();
                                    phelper.prev = prev;
                                    phelper.next = next;

                                    ReadFirstAndSecond(sr, ref first, ref second);
                                    vhelper.pairs.Add(phelper);
                                }
                            }
                            whelper.variations.Add(vhelper);
                        }
                    }
                    pos = ReturnArrPlace(whelper.asciiword);
                    arr[pos].Add(whelper);
                }
            }
        }

        public void SaveDict()
        {
            int j = 0;
            using (var sw = new StreamWriter("dict"))
            {
                for (int i = 0; i < 26; i++)
                {
                    foreach (Word whelper in arr[i])
                    {
                        sw.WriteLine(whelper.asciiword);
                        foreach (Variation v in whelper.variations)
                        {
                            sw.WriteLine("\t" + v.utfword);
                            foreach (Pair p in v.pairs)
                            {
                                sw.WriteLine("\t" + "\t" + p.prev + "|" + p.next);
                                j++;
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("Ulozeno " + j + " variaci!");
        }
    }
}
