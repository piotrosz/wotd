using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

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

            ShowWords(date);
        }

        private static void ShowWords(DateTime date)
        {
            var results = new HtmlScrapper().GetWords(new HtmlDownloader().GetHtml(date));

            foreach (var word in results)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(word.Word);
                Console.ResetColor();
                Console.WriteLine(" ({0})", word.Index);
                Console.Write(word.Comment);
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]", programName);
            Console.WriteLine("Opis...");
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
