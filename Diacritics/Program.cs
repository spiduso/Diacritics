using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Diacritics
{
    class Program
    {
        static void EditLinesToSkip(int linestoskip)
        {
            using (var ls = new StreamReader(@"linestoskip"))
            {
                linestoskip += Convert.ToInt32(ls.ReadLine());
            }

            using (var ls = new StreamWriter(@"linestoskip"))
            {
                ls.WriteLine(linestoskip);
            }
        }

        static void MakeSentencesFromCorpus(int wordcount)
        {
            int linestoskip;
            string word;

            /// <summary>
            /// 0 - First word
            /// 1 - word
            /// 2 - non-word
            /// </summary>
            byte type = 0;

            using (var ls = new StreamReader(@"linestoskip"))
            {
                linestoskip = Convert.ToInt32(ls.ReadLine());
            }

            using (var sr = new StreamReader(@"corpus", Encoding.UTF8))
            {
                using (var sw = new StreamWriter(@"Senteces"))
                {
                    for (int i = 0; i < linestoskip; i++)
                        sr.ReadLine();

                    word = ReadWordFromCorpusToSentences(sr);
                    sw.Write(word);
                    type = 1;

                    for (int i = 0; i < wordcount; i++)
                    {
                        word = ReadWordFromCorpusToSentences(sr);
                        if (word == "," || word == "!" || word == "?" || word == ".")
                        {
                            type = 2;
                            sw.Write(word + " ");
                        }
                        else
                        {
                            if (type == 2)
                            {
                                sw.Write(word);
                                type = 1;
                            }
                            else
                                sw.Write(" " + word);
                        }
                    }
                }
            }
        }

        static string ReadWordFromCorpusToSentences(StreamReader sr)
        {
            string word = "";
            int c = sr.Read();

            //    60 '<'
            while (c == 60)
            {
                sr.ReadLine();
                c = sr.Read();
            }

            //    9 TAB
            while (c != 9)
            {
                word += Convert.ToChar(c);
                c = sr.Read();
            }

            sr.ReadLine();
            return word.ToLower();
        }

        static void MakeDictFromSentences(Dictionary dict, string count, System.Diagnostics.Stopwatch watch)
        {
            string word;
            string next = "";
            string prev = "";

            int pocet = 0;
            int ctvrtiny = 0;

            using (var sw = new StreamWriter(@"system.txt"))
            {
                using (var sr = new StreamReader(@"Senteces", Encoding.UTF8))
                {
                    word = ReadWordFromSentences(sr).ToLower();
                    while (!sr.EndOfStream)
                    {
                        next = ReadWordFromSentences(sr).ToLower();
                        dict.AddWordToDict(word, prev, next);
                        sw.WriteLine(prev + " " + word + " " + next);
                        prev = word;
                        word = next;
                        next = "";


                        pocet++;
                        if (pocet % 2500 == 0)
                        {
                            ctvrtiny++;
                            Console.WriteLine(ctvrtiny * 2500 + ": " + watch.ElapsedMilliseconds);
                            watch.Restart();
                        }
                    }

                }
            }
        }

        static string ReadWordFromSentences(StreamReader sr)
        {
            string word = "";
            int c = sr.Read();

            while (((c > 31 && c < 48) || (c > 57 && c < 64) || (c > 90 && c < 97) || (c > 122 && c < 128) || (c > 47 && c < 58) || c == 8212 || c == 8220 || c == 8222 || c == 8230) && !sr.EndOfStream)
            {
                c = sr.Read();
            }

            while (c != 32 && c != 33 && c != 34 && c != 44 && c != 46 && c != 63 && c != 8212 && c != 8220 && c != 8222 && c != 8230 && !sr.EndOfStream)
            {
                word += Convert.ToChar(c);
                c = sr.Read();
            }

            if (sr.EndOfStream)
                if (c != 32 && c != 33 && c != 34 && c != 44 && c != 46 && c != 63 && c != 8212 && c != 8220 && c != 8222 && c != 8230)
                    word += Convert.ToChar(c);

            return word.ToLower();
        }

        static string ReadFromInput(StreamReader sr, StreamWriter sw, ref string tail, ref string head, ref string partofword)
        {
            string word = "";
            int c = sr.Read();

            while (((c > 32 && c < 64) || (c > 90 && c < 97) || (c > 122 && c < 128)) && !sr.EndOfStream)
            {
                head = "";
                head += Convert.ToChar(c);
                c = sr.Read();
            }


            while (!((c > 31 && c < 48) || (c > 57 && c < 64) || (c > 90 && c < 97) || (c > 122 && c < 128)) && !sr.EndOfStream)
            {
                word += Convert.ToChar(c);
                c = sr.Read();
            }

            if (!sr.EndOfStream)
            {
                tail = "";
                tail += Convert.ToChar(c);
                c = sr.Read();
                while (((c > 31 && c < 64) || (c > 90 && c < 97) || (c > 122 && c < 128)) && !sr.EndOfStream)
                {
                    tail += Convert.ToChar(c);
                    c = sr.Read();
                }
                partofword = "";
                partofword += Convert.ToChar(c);
            }
            else
            {
                if (((c > 32 && c < 64) || (c > 90 && c < 97) || (c > 122 && c < 128)))
                {
                    tail = "";
                    tail += Convert.ToChar(c);
                }
                else
                {
                    word += Convert.ToChar(c);
                }
            }

            return word.ToLower();
        }

        static void Show(Dictionary dict)
        {
            Console.WriteLine("--- SHOW MODE ---");
            Console.WriteLine("write the word to show variations and pairs");
            string word = Console.ReadLine();
            switch (word)
            {
                case "":
                    break;
                default:
                    Word w = dict.FindWord(word);
                    foreach (Variation v in w.variations)
                    {
                        Console.WriteLine(v.utfword);
                        foreach (Pair p in v.pairs)
                            Console.WriteLine(" " + p.prev + "  " + p.next);
                    }

                    break;
            }
        }

        static void AddFile(int numberofrows)
        {
            using (var sr = new StreamReader(@"corpus", Encoding.UTF8))
            {
                using (var sw = new StreamWriter(@"CorpusOf" + numberofrows))
                {
                    for (int i = 0; i < numberofrows; i++)
                    {
                        sw.WriteLine(sr.ReadLine());
                    }
                }
            }
        }

        static string StartOperation(Dictionary dict, string word, string prev, string next)
        {
            Word helper = dict.FindWord(word);
            Variation best = new Variation();
            int bestscore = -1;
            int currentscore = 0;
            int count = -1;
            if (helper != null)
            {
                foreach (Variation v in helper.variations)
                {
                    foreach (Pair p in v.pairs)
                    {
                        if (next == p.next)
                            currentscore++;
                        if (prev == p.prev)
                            currentscore++;

                        if ((currentscore > bestscore) || (currentscore == bestscore && v.count > count))
                        {
                            bestscore = currentscore;
                            currentscore = 0;
                            best = v;
                            count = v.count;
                        }
                    }
                }
                return best.utfword.ToLower();
            }
            return word.ToLower();
        }

        static void InsertDiacritics(Dictionary dict)
        {
            string word;
            string next = "";
            string prev = "";
            string tail = "\\/";
            string tail2 = "\\/";
            string head = "\\/";
            string partofword = "";

            using (var sw = new StreamWriter(@"output.txt"))
            {
                using (var sr = new StreamReader(@"input.txt", Encoding.UTF8))
                {
                    word = ReadFromInput(sr, sw, ref tail, ref head, ref partofword).ToLower();
                    if (head != "\\/")
                    {
                        sw.Write(head.ToLower());
                        head = "\\/";
                    }

                    while (!sr.EndOfStream)
                    {
                        next = partofword + next;
                        next += ReadFromInput(sr, sw, ref tail2, ref head, ref partofword).ToLower();
                        sw.Write(StartOperation(dict, word, prev, next));

                        if (tail != "\\/")
                            sw.Write(tail.ToLower());
                        tail = tail2.ToLower();
                        tail2 = "\\/";

                        prev = word.ToLower();
                        word = next.ToLower();
                        next = "";
                    }
                    sw.Write(StartOperation(dict, word, prev, next));
                    if (tail != "\\/")
                        sw.Write(tail);
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Vitejte v programu pro doplnovani diakritiky");
            Console.WriteLine("Predtim, nez zacneme, vlozte soubor, ktery chcete ohackovat, pote zmacknete klavesu");
            Console.ReadKey();
            Console.WriteLine();
            Dictionary dict = new Dictionary();

            Console.WriteLine("Probiha nacitani slovniku");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            dict.LoadDict();
            Console.WriteLine("Nacteni dokonceno, trvalo to " + watch.ElapsedMilliseconds + " ms");
            watch.Reset();
            Console.WriteLine();

            Console.WriteLine("Zadejte pocet slov, ktere se zpracuji z korpusu");
            int howmanywords = Convert.ToInt32(Console.ReadLine());

            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            if (howmanywords < 0)
                Console.WriteLine("blbe");
            else if (howmanywords > 0)
            {
                watch.Start();
                MakeSentencesFromCorpus(howmanywords);
                Console.WriteLine("Nacteni vet z korpusu trvalo " + watch.ElapsedMilliseconds + " ms");
                watch.Restart();
                MakeDictFromSentences(dict, howmanywords.ToString(), watch);
                Console.WriteLine("A slova z vet se nahraly za " + watch2.ElapsedMilliseconds + " ms");
                EditLinesToSkip(howmanywords);
            }
            watch.Stop();
            watch2.Restart();

            InsertDiacritics(dict);
            Console.WriteLine();
            Console.WriteLine("A konecne! Doplneni diakritiky trvalo : " + watch2.ElapsedMilliseconds + " ms");

            watch2.Restart();
            dict.SaveDict();
            Console.WriteLine("Nakonec slovnik ulozen za : " + watch2.ElapsedMilliseconds + " ms");
            watch2.Stop();
        }
    }
}
