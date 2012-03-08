using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace wotd
{
    public class Cache
    {
        private Dictionary<DateTime, List<WordInfo>> cache;

        string path = "words.xml";

        public Cache()
        {
            if (!File.Exists(path))
                File.Create(path);

            Deserialize();
        }

        public void Serialize()
        {
             var serializer = new XmlSerializer(typeof(Dictionary<DateTime, List<WordInfo>>));

            using (TextWriter writeFileStream = new StreamWriter(path))
            {
                serializer.Serialize(writeFileStream, cache);
                writeFileStream.Close();
            } 
        }

        private void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(Dictionary<DateTime, List<WordInfo>>));

            using (var reader = new StreamReader(path))
            {
                cache = (Dictionary<DateTime, List<WordInfo>>)serializer.Deserialize(reader);
                reader.Close();
            }
        }

        public List<WordInfo> Get(DateTime date)
        {
            if(cache.ContainsKey(date))
                return cache[date];
            else
                return null;
        }

        public void Store(List<WordInfo> words, DateTime date)
        {
             cache[date] = words;
        }
    }
}
