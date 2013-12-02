using System;
using System.Diagnostics;

namespace wotd
{
    [Serializable]
    [DebuggerDisplay("{Word}")]
    public class WordInfo
    {
        public string Word { get; set; }
        public string Index { get; set; }
        public string Comment { get; set; }
    }
}
