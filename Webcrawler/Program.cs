using WebCrawler;


/*
var linkFinder = new LinkFinder();
var links = linkFinder.FindLinks();
foreach (var link in links)
{
    Console.WriteLine(link);
    
}
*/


/*// Create an instance of the LinkFinder class
var linkFinder = new LinkFinder();

// Retrieve the links from the HTML document
var links = linkFinder.FindLinks();

// Create an instance of the WebsiteDownloader class
var websiteDownloader = new WebsiteDownloader();*/

// Download the website using the links and the destination folder
/*Console.WriteLine("Enter the destination folder for the website download: ");
var destinationFolder = Console.ReadLine();
websiteDownloader.DownloadWebsite(links);*/


var linkParser = new LinkParser();
var linkChecker = new LinkChecker();
var linkFinder = new LinkFinder(linkParser, linkChecker);

IEnumerable<string> links = linkFinder.FindLinks();

foreach (var link in links)
{
    Console.WriteLine(link);
}