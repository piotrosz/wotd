using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NDesk.Options;

namespace wotd
{
    class Program
    {
        const string ProgramName = "";

        // This is the earliest date for which data is available
        static readonly DateTime dateEarliest = new DateTime(2010, 12, 11);

        static readonly HtmlScrapper Scrapper = new HtmlScrapper();
        static readonly HtmlDownloader Downloader = new HtmlDownloader();
        static readonly Cache Cache = new Cache();

        static DateTime _date = DateTime.Today.AddDays(-1);
        static bool _dateParsed;
        static bool _showHelp = false;
        static string _wordToFind = "";
        static bool _searchInDescriptions = false;

        const int LevenshteinDistance = 2;

        static void Main(string[] args)
        {
            var options = CreateOptions();

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", ProgramName);
                Console.WriteLine(e.Message);
                Console.WriteLine("Try '{0} --help' for more information.", ProgramName);
                return;
            }

            if (_showHelp)
            {
                ShowHelp(options);
                return;
            }

            if (string.IsNullOrWhiteSpace(_wordToFind))
                ShowWords(_date);
            else
                SearchForWord(_wordToFind, dateEarliest, _searchInDescriptions);
        }

        private static OptionSet CreateOptions()
        {
            return new OptionSet
            {
                { 
                    "d|date=",
                    "The {DATE} in yyyy-MM-dd format.",
                    v => 
                    {
                        var temp = new DateTime();
                        _dateParsed = DateTime.TryParse(v, out temp);
                        if (_dateParsed)
                        {
                            _date = temp;
                        }
                    }
                },
                {
                    "e|description",
                    "Search in descriptions also",
                    v => _searchInDescriptions = v != null
                },
                {
                    "f|find=",
                    "The {WORD} to find.",
                    v => _wordToFind = v
                },
                { 
                    "h|help",  
                    "show this message and exit", 
                    v => _showHelp = v != null 
                }
            };
        }

        private static void ShowWords(DateTime date)
        {
            List<WordInfo> results = Cache.Get(date);

            if (results == null)
            {
                results = Scrapper.GetWords(Downloader.GetHtml(date));

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
                    List<WordInfo> results = Cache.Get(date);

                    if (results == null)
                    {
                        results = Scrapper.GetWords(Downloader.GetHtml(date));
                        StoreInCache(results, date);
                    }

                    Console.Write("\rChecking for: {0:yyyy-MM-dd}", date);

                    foreach (var result in results)
                    {
                        if (
                                result.Word.Contains(word, StringComparison.InvariantCultureIgnoreCase) ||
                                (searchInDescriptions && result.Comment.ContainsSimilar(word, LevenshteinDistance))
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
                Cache.Store(words, date);
                Cache.Serialize();
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
                var indexOfAll = word.Comment.IndexOfAllSimilar(searchTerm, LevenshteinDistance).ToList();

                for (int i = 0; i < word.Comment.Length; i++)
                {
                    var found = indexOfAll.FirstOrDefault(x => x.Item1 == i);

                    if (found != null)
                    {
                        int length = found.Item2.Length;
                        PrintGreen(word.Comment.Substring(i, length));
                        i += (length - 1);
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
            Console.WriteLine("Usage: {0} [OPTIONS]", ProgramName);
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
