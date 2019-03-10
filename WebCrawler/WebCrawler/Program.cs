using System;
using System.Collections.Generic;
using Models;
using Models.Interfaces;

namespace WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var crawlers = new Dictionary<string, ICrawler>();
            var vnExpressCrawler = new VnExpressCrawler();
            crawlers.Add(vnExpressCrawler.host, vnExpressCrawler);

            if (args.Length > 0)
            {
                try
                {
                    var url = args[0];
                    Console.WriteLine($"---------------------------------------------------");
                    Console.WriteLine($"Url: {url}");
                    Uri myUri = new Uri(url);
                    string host = myUri.Host;
                    if (crawlers.ContainsKey(host))
                    {
                        ICrawler crawler = crawlers[host];
                        var dic = new Dictionary<string, ICrawlerResult>();
                        crawler.Crawl(dic, url, 20);
                        crawler.ExportToFile(dic);
                    }
                    else Console.WriteLine($"Not support this host: {host}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

            }
            else Console.WriteLine($"Please input url");
        }
    }
}
