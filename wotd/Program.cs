using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using NDesk.Options;

namespace wotd
{
    class Program
    {
        const string programName = "";

        static HtmlScrapper scrapper = new HtmlScrapper();
        static HtmlDownloader downloader = new HtmlDownloader();
        static Cache cache = new Cache();

        static void Main(string[] args)
        {
            DateTime date = DateTime.Today.AddDays(-1);
            bool dateParsed;
            bool showHelp = false;
            string wordToFind = "";
            bool searchInDescriptions = false;

            // This is the earliest date for which data is available
            DateTime datePast = new DateTime(2010, 12, 11);

            OptionSet options = new OptionSet
            {
                { 
                    "d|date=",
                    "The {DATE} in yyyy-MM-dd format.",
                    v => 
                    {
                        DateTime temp = new DateTime();
                        dateParsed = DateTime.TryParse(v, out temp);
                        if(dateParsed)
                            date = temp;
                    }
                },
				{
					"e|description",
					"Search in descriptions also",
					v => searchInDescriptions = v != null
				},
                {
                    "f|find=",
                    "The {WORD} to find.",
                    v => wordToFind = v
                },
                { 
                    "h|help",  
                    "show this message and exit", 
                    v => showHelp = v != null 
                }
            };

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", programName);
                Console.WriteLine(e.Message);
                Console.WriteLine("Try '{0} --help' for more information.", programName);
                return;
            }

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            if (string.IsNullOrWhiteSpace(wordToFind))
                ShowWords(date);
            else
                SearchForWord(wordToFind, datePast, searchInDescriptions);
        }

        private static void ShowWords(DateTime date)
        {
            List<WordInfo> results = cache.Get(date);

            if (results == null)
            {
                results = scrapper.GetWords(downloader.GetHtml(date));

                StoreInCache(results, date);
            }

            foreach (var word in results)
                Print(word, null);
        }

        private static void SearchForWord(string word, DateTime datePast, bool searchInDescriptions)
        {
            var date = DateTime.Today;

            while (date >= datePast)
            {
                try
                {
                    List<WordInfo> results = cache.Get(date);

                    if (results == null)
                    {
                        results = scrapper.GetWords(downloader.GetHtml(date));

                        StoreInCache(results, date);
                    }

                    Console.Write("\rChecking for: {0:yyyy-MM-dd}", date);

                    foreach (var result in results)
                    {
                        if (
                                result.Word.Contains(word, StringComparison.InvariantCultureIgnoreCase) ||
                                (searchInDescriptions && result.Comment.Contains(word, StringComparison.InvariantCultureIgnoreCase))
                            )
                        {
                            Console.WriteLine();
                            Print(result, word);
                        }
                    }
                    date = date.AddDays(-1);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }
        }

        static void StoreInCache(List<WordInfo> words, DateTime date)
        {
            if (words.Count > 0)
            {
                cache.Store(words, date);
                cache.Serialize();
            }
        }

        static void Print(WordInfo word, string searchTerm)
        {
            PrintGreen(word.Word);

            Console.WriteLine(" ({0})", word.Index);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.Write(word.Comment);
            }
            else
            {
                var indexOfAll = word.Comment.IndexOfAll(searchTerm);

                for (int i = 0; i < word.Comment.Length; i++)
                {
                    if (indexOfAll != null && indexOfAll.Any(x => x == i))
                    {
                        PrintGreen(word.Comment.Substring(i, searchTerm.Length));
                        i += (searchTerm.Length - 1);
                    }
                    else
                    {
                        Console.Write(word.Comment.Substring(i, 1));
                    }

                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        static void PrintGreen(string word)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(word);
            Console.ResetColor();
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]", programName);
            Console.WriteLine("Shows Polish words of the day. See www.nkjp.pl for more information.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static void ShowException(Exception ex)
        {
            Console.WriteLine("Exception occured:");
            Console.WriteLine(ex.ToString());
        }
    }
}
