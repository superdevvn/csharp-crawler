# Web Crawler

[Sendo Test] Web Crawler 

## Run

```bash
Go to cmd folder
```

```bash
Open terminal
```

```bash
Example: webcrawler https://vnexpress.net/the-gioi/may-bay-cho-189-nguoi-roi-xuong-bien-o-indonesia-3830821.html
```

```bash
Check csv file
```

## Usage

Using Visual Studio to open Solution

## Explanation

Interface **ICrawlerResult**
```c#
public interface ICrawler
{
    string host { get; }
    void Crawl(Dictionary<string, ICrawlerResult> dic, string url, int limit);
    void ExportToFile(Dictionary<string, ICrawlerResult> dic, string filePath = null);
}
```

Interface **ICrawler**
```c#
public interface ICrawler
{
    string host { get; }
    void Crawl(Dictionary<string, ICrawlerResult> dic, string url, int limit);
    void ExportToFile(Dictionary<string, ICrawlerResult> dic, string filePath = null);
}
```

Class **CrawlerResult**
```c#
public class CrawlerResult: ICrawlerResult
{
    public string Url { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string Date { get; set; }

    public string Content { get; set; }
}
```

Abstract Class **Crawler**
```c#
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
```

Class **VnExpressCrawler** interhit from **Crawler** to crawl url from **vnexpress.net**
```c#
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
            crawlerResult.Title = document.DocumentNode.Descendants("h1").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("title_news_detail mb10")).InnerText.Trim();

            // Get Content
            crawlerResult.Content = document.DocumentNode.Descendants("article").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("content_detail fck_detail width_common block_ads_connect")).InnerText.Trim();

            // Get Author
            crawlerResult.Author = document.DocumentNode.Descendants("p").First(e => e.Attributes.Contains("class") && e.Attributes["class"].Value.Contains("author_mail")).Descendants("strong").First().InnerText.Trim();

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
```

Push all **ICrawler** to dictionary with **host** is the key
```c#
var crawlers = new Dictionary<string, ICrawler>();
var vnExpressCrawler = new VnExpressCrawler();
crawlers.Add(vnExpressCrawler.host, vnExpressCrawler);
```

Read value of url from args[0] and get **ICrawler** base on host and crawl it
```c#
if (args.Length > 0)
{
    try
    {
        var url = args[0];
        Console.WriteLine($"---------------------------------------------------");
        Console.WriteLine($"Url: {args[0]}");
        Uri myUri = new Uri(url);
        string host = myUri.Host;
        if (crawlers.ContainsKey(host))
        {
            ICrawler crawler = crawlers[host];
            var dic = new Dictionary<string, ICrawlerResult>();
            crawler.Crawl(dic, args[0], 10);
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
```