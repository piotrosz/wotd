using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wotd
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static IEnumerable<int> IndexOfAll(this string source, string value)
        {
            for (int i = 0; i < (source.Length - value.Length + 1); i++)
            {
                if (source.IndexOf(value, i, value.Length, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    yield return i;
                }
            }
        }
    }
}
