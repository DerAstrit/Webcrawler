
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace WebCrawler
{
    public class WebsiteDownloader
    {
        public void DownloadWebsite(string url)
        {
            
            Console.WriteLine("Enter the destination folder for the website download: ");
            var destinationFolder = Console.ReadLine();

            // Create the destination folder if it doesn't exist
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            // Download the homepage HTML
            var homepageHtml = DownloadHtml(url);

            // Save the homepage HTML to a file
            var homepageHtmlPath = Path.Combine(destinationFolder, "index.html");
            File.WriteAllText(homepageHtmlPath, homepageHtml, Encoding.UTF8);

            // Load the homepage HTML into an HtmlDocument
            var doc = new HtmlDocument();
            doc.LoadHtml(homepageHtml);

            // Extract the links to CSS and image files from the homepage HTML
            var cssLinks = ExtractLinks(doc, "link", "href", "stylesheet");
            var imageLinks = ExtractLinks(doc, "img", "src");

            // Download the CSS and image files
            DownloadFiles(url, cssLinks, destinationFolder, "css");
            DownloadFiles(url, imageLinks, destinationFolder, "img");
        }

        private string DownloadHtml(string url)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        private IEnumerable<string> ExtractLinks(HtmlDocument doc, string tag, string attribute, string value = null)
        {
            return doc.DocumentNode.Descendants(tag)
                .Where(node => node.HasAttributes && node.Attributes.Contains(attribute))
                .Select(node => node.Attributes[attribute].Value)
                .Where(link => value == null || link.Contains(value));
        }

        private void DownloadFiles(string baseUrl, IEnumerable<string> links, string destinationFolder, string subfolder)
        {
            // Create the subfolder if it doesn't exist
            var subfolderPath = Path.Combine(destinationFolder, subfolder);
            if (!Directory.Exists(subfolderPath))
            {
                Directory.CreateDirectory(subfolderPath);
            }

            // Download each file
            foreach (var link in links)
            {
                var fileUrl = new Uri(new Uri(baseUrl), link).AbsoluteUri;
                var fileName = Path.GetFileName(link);
                if (string.IsNullOrEmpty(fileName))
                {
                    // If the link does not represent a file, use a unique ID as the file name
                    fileName = Guid.NewGuid().ToString();
                }
                var filePath = Path.Combine(subfolderPath, fileName);


                using (var client = new WebClient())
                {
                    client.DownloadFile(fileUrl, filePath);
                }
            }
        }


        
    }
}