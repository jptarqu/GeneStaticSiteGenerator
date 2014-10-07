using Abot.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Core;
using WebsiteCrawler.GenericSite;
using WebsiteCrawler.Sharepoint;

namespace WebsiteCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            CrawlRequest request = new CrawlRequest() { EntryURL = @"http://bootswatch.com/darkly/" };// @"http://localhost:50665/" };
            GenericSiteCrawler crawler = new GenericSiteCrawler();
            crawler.Crawl(request);
        }

    }
}
