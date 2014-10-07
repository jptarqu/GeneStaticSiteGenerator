using Abot.Crawler;
using Abot.Poco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Core;
using WebsiteCrawler.Sharepoint;

namespace WebsiteCrawler.GenericSite
{
    public class GenericSiteCrawler
    {
        public void Crawl(CrawlRequest request)
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            crawlConfig.CrawlTimeoutSeconds = 100;
            crawlConfig.MaxConcurrentThreads = 10;
            crawlConfig.MaxPagesToCrawl = 100;
            crawlConfig.UserAgentString = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; abot v1.0 http://code.google.com/p/abot)";
            crawlConfig.ConfigurationExtensions.Add("SomeCustomConfigValue1", "1111");
            crawlConfig.ConfigurationExtensions.Add("SomeCustomConfigValue2", "2222");
            crawlConfig.MaxCrawlDepth = 10;
            crawlConfig.DownloadableContentTypes = "text/html, text/plain";
            crawlConfig.IsHttpRequestAutoRedirectsEnabled = true;
            crawlConfig.HttpRequestMaxAutoRedirects = 100;
            crawlConfig.IsExternalPageCrawlingEnabled = false;

            //Will use the manually created crawlConfig object created above
            PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null,null);

            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            CrawlResult result = crawler.Crawl(new Uri(request.EntryURL));

            if (result.ErrorOccurred)
                    Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            else
                    Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            var contents = crawledPage.Content.Text;
            var filename = crawledPage.Uri.LocalPath;//.ToString().Replace(crawledPage.Uri.AbsoluteUri, "");
            filename = SavePage(contents, filename);

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
        }

        private static string SavePage(string contents, string filename)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            
            foreach (char c in invalid)
            {
                if (c != '/')
                {
                    filename = filename.Replace(c, '~');
                }
            }

            filename = '.' + filename; //make the file relative to bin

            if (filename.Last() == '/')
            {
                filename += "index.html";
            }

            var folder_path = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(folder_path))
            {
                Directory.CreateDirectory(folder_path);
            }

            //if (string.IsNullOrEmpty(filename))
            //{
            //    filename = "index";
            //}
            if (!File.Exists(filename))
            {
                File.WriteAllText(filename, contents);
            }
            return filename;
        }

        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
