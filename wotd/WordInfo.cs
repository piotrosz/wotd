using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
