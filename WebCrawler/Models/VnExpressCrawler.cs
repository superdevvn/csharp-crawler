using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Models.Interfaces;

namespace Models
{
    public class VnExpressCrawler : Crawler
    {
        public override string host { get => "vnexpress.net"; }
        public override void Crawl(Dictionary<string, ICrawlerResult> dic, string url, int limit)
        {
            try
            {
                if (dic.Count == limit) return;

                // Kiểm tra có phải link bài viết của vnexpress hay không?
                if (!url.Contains(host)) return;
                if (url.IndexOf(".html") == -1) return;
                url = url.Substring(0, url.IndexOf(".html") + 5);

                // Kiểm tra url đã Craw hay chưa?
                if (dic.ContainsKey(url)) return;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"Status Code: {response.StatusCode}");

                string html = string.Empty;

                using (var stream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        html = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                    response.Close();
                }

                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(html);

                ICrawlerResult crawlerResult = new CrawlerResult();

                // Url
                crawlerResult.Url = url;

                // Get Title
                crawlerResult.Title = document.DocumentNode.Descendants("h1").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("title_news_detail")).InnerText.Trim();

                // Get Content
                crawlerResult.Content = document.DocumentNode.Descendants("article").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("content_detail")).InnerText.Trim();

                // Get Author
                var authorInDetail = document.DocumentNode.Descendants("article").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("content_detail")).Descendants("p").LastOrDefault(e=> e.Attributes.Contains("style") && e.Attributes["style"].Value.Contains("text-align:right;"));
                if(authorInDetail != null)
                {
                    crawlerResult.Author = authorInDetail.Descendants("strong").First().InnerHtml.Trim();
                } else crawlerResult.Author = document.DocumentNode.Descendants("p").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("author_mail")).Descendants("strong").First().InnerText.Trim();

                // Get Date
                crawlerResult.Date = document.DocumentNode.Descendants("span").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("time left")).InnerText.Trim();

                dic.Add(url, crawlerResult);
                var elements = document.DocumentNode.Descendants("a").Where(e => e.Attributes.Contains("href") && e.Attributes["href"].Value.Contains(host));
                foreach (var element in elements)
                {
                    var href = element.Attributes["href"].Value.Trim();
                    Crawl(dic, element.Attributes["href"].Value.Trim(), limit);
                }
            }
            catch
            {
                return;
            }
        }
        public override void ExportToFile(Dictionary<string, ICrawlerResult> dic, string filePath = null)
        {
            if(filePath == null) filePath = $"[{host}] {Guid.NewGuid().ToString()}.csv";
            base.ExportToFile(dic, filePath);
        }
    }
}
