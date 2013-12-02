using System;
using System.Collections.Generic;

namespace wotd
{
    [Serializable]
    public class WordsOfTheDay
    {
        public List<WordInfo> Words { get; set; }
        public DateTime Date { get; set; }
    }
}
