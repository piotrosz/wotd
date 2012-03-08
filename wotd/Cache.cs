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
        private Type T = typeof(List<WordsOfTheDay>);

        private List<WordsOfTheDay> cache = new List<WordsOfTheDay>();

        string path = "words.xml";

        public Cache()
        {
            if (!File.Exists(path))
                File.Create(path);
            
            if(!string.IsNullOrWhiteSpace(File.ReadAllText(path)))
                Deserialize();
        }

        public void Serialize()
        {
             var serializer = new XmlSerializer(T);

            using (TextWriter writeFileStream = new StreamWriter(path))
            {
                serializer.Serialize(writeFileStream, cache);
                writeFileStream.Close();
            } 
        }

        private void Deserialize()
        {
            var serializer = new XmlSerializer(T);

            using (var reader = new StreamReader(path))
            {
                cache = (List<WordsOfTheDay>)serializer.Deserialize(reader);
                reader.Close();
            }
        }

        public List<WordInfo> Get(DateTime date)
        {
            var result = cache.SingleOrDefault(x => x.Date.Date == date.Date);

            if (result == null)
                return null;
            else
                return result.Words;

        }

        public void Store(List<WordInfo> words, DateTime date)
        {
            cache.Add(new WordsOfTheDay { Date = date, Words = words });
        }
    }
}
