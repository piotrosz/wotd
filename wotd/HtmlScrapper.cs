using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace wotd
{
    public class HtmlScrapper
    {
        public List<WordInfo> GetWords(string html)
        {
            var r1 = new Regex(@"<td class=""fifteen"" width=""150px"">\s([\w]+)</td>");
            var r2 = new Regex(@"<td class=""fifteen"" width=""25px"">\s([0-9]+:[0-9]+)</td>");
            var r3 = new Regex(@"<td class=""fifteen"">\s([^<>]+)</td>");

            var words = new List<string>();
            var importance = new List<string>();
            var comments = new List<string>();

            for (Match match = r1.Match(html); match.Success; match = match.NextMatch())
            {
                words.Add(match.Groups[1].ToString());
            }

            for (Match match = r2.Match(html); match.Success; match = match.NextMatch())
            {
                importance.Add(match.Groups[1].ToString());
            }

            for (Match match = r3.Match(html); match.Success; match = match.NextMatch())
            {
                comments.Add(match.Groups[1].ToString());
            }

            var result = new List<WordInfo>();

            if (words.Count == 0 || importance.Count == 0 || comments.Count == 0)
            {
                return result;
            }

            for (int i = 0; i < words.Count; i++)
            {
                string word = i < words.Count ? words[i] : "";
                string index = i < importance.Count ? importance[i] : "";
                string comment = i< comments.Count ? comments[i] : "";

                result.Add(new WordInfo 
                { 
                    Word = word,
                    Index = index,
                    Comment = comment
                });
            }

            return result;
        }
    }
}
