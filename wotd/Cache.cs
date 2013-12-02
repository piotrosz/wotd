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
        private readonly Type T = typeof(List<WordsOfTheDay>);

        private List<WordsOfTheDay> _cache = new List<WordsOfTheDay>();

        private const string Path = "words.xml";

        public Cache()
        {
            if (!File.Exists(Path))
            {
                File.Create(Path);
            }
            
            if (!string.IsNullOrWhiteSpace(File.ReadAllText(Path)))
            {
                Deserialize();
            }
                
        }

        public void Serialize()
        {
             var serializer = new XmlSerializer(T);

            using (TextWriter writeFileStream = new StreamWriter(Path))
            {
                serializer.Serialize(writeFileStream, _cache);
                writeFileStream.Close();
            } 
        }

        private void Deserialize()
        {
            var serializer = new XmlSerializer(T);

            using (var reader = new StreamReader(Path))
            {
                _cache = (List<WordsOfTheDay>)serializer.Deserialize(reader);
                reader.Close();
            }
        }

        public List<WordInfo> Get(DateTime date)
        {
            var result = _cache.SingleOrDefault(x => x.Date.Date == date.Date);

            if (result == null)
            {
                return null;
            }
                
            return result.Words;
        }

        public void Store(List<WordInfo> words, DateTime date)
        {
            _cache.Add(new WordsOfTheDay { Date = date, Words = words });
        }
    }
}
