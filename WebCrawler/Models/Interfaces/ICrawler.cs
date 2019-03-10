using System.Collections.Generic;

namespace Models.Interfaces
{
    public interface ICrawler
    {
        string host { get; }
        void Crawl(Dictionary<string, ICrawlerResult> dic, string url, int limit);
        void ExportToFile(Dictionary<string, ICrawlerResult> dic, string filePath = null);
    }
}
