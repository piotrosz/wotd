using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Net;

namespace wotd
{
    class Program
    {
        const string programName = "";

        static void Main(string[] args)
        {
            DateTime date = DateTime.Today.AddDays(-1);
            bool dateParsed;
            bool showHelp = false;
            string wordToFind = "";

            // TODO: Add it as a command line parameter.
            DateTime datePast = DateTime.Today.AddYears(-1);

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
                SearchForWord(wordToFind, datePast);
        }

        private static void ShowWords(DateTime date)
        {
            var results = new HtmlScrapper().GetWords(new HtmlDownloader().GetHtml(date));

            foreach (var word in results)
            {
                Print(word);
            }
        }

        // TODO: Debug cache

        private static void SearchForWord(string word, DateTime datePast)
        {
            DateTime date = DateTime.Today;
            var scrapper = new HtmlScrapper();
            var htmlDonwloader = new HtmlDownloader();
            var cache = new Cache();

            while (date >= datePast)
            {
                try
                {
                    List<WordInfo> results = new List<WordInfo>();

                    List<WordInfo> cachedResults = cache.Get(date);

                    if (cachedResults.Count == 0)
                    {
                        results = scrapper.GetWords(htmlDonwloader.GetHtml(date));
                        cache.Store(results, date);
                    }
                    else
                        results = cachedResults;

                    Console.Write("\rChecking for: {0:yyyy-MM-dd}", date);
                    foreach (var result in results)
                    {
                        if (result.Word.Contains(word, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine();
                            Print(result);
                        }
                    }
                    date = date.AddDays(-1);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.ToString());
                    cache.Serialize();
                    break;
                }
            }

            cache.Serialize();
        }

        static void Print(WordInfo word)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(word.Word);
            Console.ResetColor();
            Console.WriteLine(" ({0})", word.Index);
            Console.Write(word.Comment);
            Console.WriteLine();
            Console.WriteLine();
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
