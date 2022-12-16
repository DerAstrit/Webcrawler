using System.Diagnostics;
using System.Net;

namespace Webcrawler;

public class SaveWebsite
{
    public static void getLinks()
    {
        // URL: http://en.wikipedia.org/wiki/Main_Page
        
        WebClient w = new WebClient();
        Console.WriteLine("Bitte geben sie einen gültigen Link ein.");
        
        string url = Console.ReadLine();
        while (UrlValidation(url) == false)
        {
            Console.WriteLine("Bitte geben sie einen gültigen Link ein. Eingegebener Link war nicht erreichbar");
            url = Console.ReadLine();
        }

     
        string s = w.DownloadString(url);
        
        Console.WriteLine("Bitte geben sie den Dateinamen an");
        string fileName = Console.ReadLine();
        w.DownloadFile(url, fileName);
        
        foreach (LinkItem i in LinkFinder.Find(s))
        {
            Console.WriteLine(i);
        }
    }

    public static Boolean UrlValidation(string url)
    {
        try
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "HEAD";
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            {
                if (response.StatusCode.ToString() == "OK")
                {
                    return true;
                }
                return false;
            }
        }
        catch
        {
            return false;
        }
        
    }
}