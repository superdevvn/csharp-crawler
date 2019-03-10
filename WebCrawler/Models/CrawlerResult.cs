using System;
using System.Collections.Generic;
using Models.Interfaces;

namespace Models
{
    public class CrawlerResult: ICrawlerResult
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Date { get; set; }

        public string Content { get; set; }
    }
}
