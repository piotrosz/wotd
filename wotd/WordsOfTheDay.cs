using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wotd
{
    [Serializable]
    public class WordsOfTheDay
    {
        public List<WordInfo> Words { get; set; }
        public DateTime Date { get; set; }
    }
}
