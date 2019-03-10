using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Interfaces;

namespace Models
{
    public abstract class Crawler : ICrawler
    {
        public abstract string host { get; }
        public virtual void ExportToFile(Dictionary<string, ICrawlerResult> dic, string filePath = null)
        {
            if (filePath == null || !filePath.EndsWith(".csv")) filePath = $"{Guid.NewGuid().ToString()}.csv";
            var csv = new StringBuilder();
            csv.AppendLine($"URL, Title, Author, Date, Content");
            foreach (var crawlerResult in dic.Values) csv.AppendLine($"{crawlerResult.Url}, {crawlerResult.Title}, {crawlerResult.Author}, {crawlerResult.Date}, {crawlerResult.Content}");
            File.WriteAllText(filePath.Trim(), csv.ToString(), Encoding.UTF8);
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"Export To File: {filePath}");
        }

        public abstract void Crawl(Dictionary<string, ICrawlerResult> dic, string url, int limit);
    }
}
