using System;
using System.Text;
using System.Net;

namespace wotd
{
    public class HtmlDownloader
    {
        public string GetHtml(DateTime date)
        {
            string result = "";

            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                result = webClient.DownloadString(string.Format("http://nkjp.uni.lodz.pl/WordsOfDay?date_key={0}#kw", date.ToString("yyyy-MM-dd")));
            }

            return result;
        }
    }
}
