using System;
using System.Collections.Generic;

namespace Models.Interfaces
{
    public interface ICrawlerResult
    {
        string Url { get; set; }

        string Title { get; set; }

        string Author { get; set; }

        string Date { get; set; }

        string Content { get; set; }
    }
}
