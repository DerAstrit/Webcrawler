using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace WebCrawler
{
    public interface ILinkParser
    {
        IEnumerable<string> ParseLinks(string html);
    }

    public class LinkParser : ILinkParser
    {
        public IEnumerable<string> ParseLinks(string html)
        {
            var links = new ConcurrentBag<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//*[@href]");
            if (nodes != null)
            {
                Parallel.ForEach(nodes, node =>
                {
                    var link = node.Attributes["href"].Value;
                    links.Add(link);
                });
            }

            return links;
        }
    }

    public interface ILinkChecker
    {
        bool IsLinkReachable(string link);
    }

    public class LinkChecker : ILinkChecker
    {
        public bool IsLinkReachable(string link)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(link);
                request.Method = "HEAD";
                request.AllowAutoRedirect = true;
                HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class LinkFinder
    {
        private readonly string _url;
        private readonly ILinkParser _linkParser;
        private readonly ILinkChecker _linkChecker;

        public LinkFinder(ILinkParser linkParser, ILinkChecker linkChecker)
        {
            Console.WriteLine("Enter the url of the website: ");
            _url = Console.ReadLine();
            _linkParser = linkParser;
            _linkChecker = linkChecker;
        }

        public IEnumerable<string> FindLinks()
        {
            var links = new ConcurrentBag<string>();

            try
            {
                using (WebClient client = new WebClient())
                {
                    string html = client.DownloadString(_url);
                    links = new ConcurrentBag<string>(_linkParser.ParseLinks(html));
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"An error occurred while trying to access the URL '{_url}': {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return links.Where(link => _linkChecker.IsLinkReachable(link));
        }
    }
}
