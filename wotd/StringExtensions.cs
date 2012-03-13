using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wotd
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string value, StringComparison comparision)
        {
            return source.IndexOf(value, comparision) != -1;
        }

        public static bool ContainsSimilar(this string source, string value, int levenshteinDistance)
        {
            return source.Split(' ').Any(x => x.LevenshteinDistance(value) < levenshteinDistance);
        }

        public static IEnumerable<int> IndexOfAll(this string source, string value)
        {
            for (int index = 0; index < (source.Length - value.Length + 1); index++)
            {
                if (source.IndexOf(value, index, value.Length, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    yield return index;
                }
            }
        }

        public static IEnumerable<Tuple<int, string>> IndexOfAllSimilar(this string source, string value, int levenshteinDistance)
        {
            foreach (var word in source.Split(' ').Where(x => x.LevenshteinDistance(value) < levenshteinDistance))
            {
                foreach (int index in IndexOfAll(source, word))
                {
                    yield return Tuple.Create<int, string>(index, word);
                }
            }
        }

        public static int LevenshteinDistance(this string source, string value)
        {
            int n = source.Length;
            int m = value.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (value[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
