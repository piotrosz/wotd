﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace wotd
{
    public class HtmlScrapper
    {
        public List<WordInfo> GetWords(string html)
        {
            Regex r1 = new Regex(@"<td class=""fifteen"" width=""150px"">\s([\w]+)</td>");
            Regex r2 = new Regex(@"<td class=""fifteen"" width=""25px"">\s([0-9]+:[0-9]+)</td>");
            Regex r3 = new Regex(@"<td class=""fifteen"">\s([^<>]+)</td>");

            List<string> words = new List<string>();
            List<string> importance = new List<string>();
            List<string> comments = new List<string>();

            for (Match match = r1.Match(html); match.Success; match = match.NextMatch())
                words.Add(match.Groups[1].ToString());

            for (Match match = r2.Match(html); match.Success; match = match.NextMatch())
                importance.Add(match.Groups[1].ToString());

            for (Match match = r3.Match(html); match.Success; match = match.NextMatch())
                comments.Add(match.Groups[1].ToString());

            List<WordInfo> result = new List<WordInfo>();

            if (words.Count == 0 || importance.Count == 0 || comments.Count == 0)
                return result;

            for (int i = 0; i < words.Count; i++)
            {
                result.Add(new WordInfo 
                { 
                    Word = words[i],
                    Index = importance[i],
                    Comment = comments[i]
                });
            }

            return result;
        }
    }
}
