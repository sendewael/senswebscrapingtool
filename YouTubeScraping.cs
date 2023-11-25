//Sources used to create this tool/code:
//https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
//https://www.selenium.dev/documentation/webdriver/
//https://toolsqa.com/selenium-webdriver/c-sharp/webelement-commands-in-c/
//https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
//https://stackoverflow.com/questions/25683161/fastest-way-to-convert-a-list-of-objects-to-csv-with-each-object-values-in-a-new
//https://www.youtube.com/watch?v=VQU58dZEFfo
//https://en.code-bude.net/2018/02/14/send-push-notifications-c/

using System;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Text.Json;
using ServiceStack.Text;


namespace Webscraping
{
    //This class is created to be able to create 'VideoData' objects to put into a list later on.
    public class VideoData
    {
        //A VideoData object takes the following arguments and stores them in the object.
        public string Title { get; set; }
        public string Uploader { get; set; }
        public string Views { get; set; }
        public string Url { get; set; }
    }
    public class YouTubeScraping
    {
        public static void YouTubeScrape()
        {
            //I ask the user for input. The input is written to the 'searchTermYT' variable.
            Console.WriteLine("Search for videos:");
            string searchTermYT = Console.ReadLine();

            Console.WriteLine("\nSome Geckodriver stuff...");

            //This line of code starts the Firefox driver, and opens the browser.
            IWebDriver driver = new FirefoxDriver();

            //The browser opens the given url. I pass the 'searchTermYT' variable, and add a piece of code to show the most recent videos.
            driver.Url = "https://www.youtube.com/results?search_query=" + searchTermYT + "&sp=CAISAhAB";

            //The next 3 lines of code are written to press 'Ignore' on the cookie window.
            //I wait so that the cookie window is loaded before continuing. This prevents errors if the cookie window is not loaded yet.
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            //Select the 'Ignore' button by using the XPath of the button and store this in the 'cookieButton' variable of type 'IWebElement'.
            IWebElement cookieButton = driver.FindElement(By.XPath("/html/body/ytd-app/ytd-consent-bump-v2-lightbox/tp-yt-paper-dialog/div[4]/div[2]/div[6]/div[1]/ytd-button-renderer[1]/yt-button-shape/button"));
            //Click the 'Ignore' button by using the 'Click' method.
            cookieButton.Click();

            //The following piece of code adds the video title, uploader, views and link to lists.
            //I use 'Take(5)' to add the first 5 found elements.
            //The used elements are selected by using their XPath.
            List<IWebElement> videoTitles = driver.FindElements(By.XPath("//a[@id='video-title']")).Take(5).ToList();
            List<IWebElement> videoUploader = driver.FindElements(By.XPath("//yt-formatted-string[@class='style-scope ytd-channel-name']//a")).Take(5).ToList();
            List<IWebElement> videoViews = driver.FindElements(By.XPath("//*[@id=\"metadata-line\"]/span[1]")).Take(5).ToList();
            //I take the first 6 found elements of the link instead of 5. I do this because there is an add displayed above to videos (1 add URL, 5 video URLs).
            //Urls can be wrong sometimes due to YouTube showing 'Shorts' or adds between videos.
            List<IWebElement> videoUrl = driver.FindElements(By.XPath("//a[@id='thumbnail']")).Take(6).ToList();

            //Just some text to make it prettier.
            Console.WriteLine("\nThis data is found for '" + searchTermYT + "' on YouTube:");

            //I create a new list 'videoList' that takes objects of type 'VideoData'. This list will contain the 5 videos with their corresponding data.
            //I create this list to be able to convert it to .JSON and .CSV in a proper and easy way.
            List<VideoData> videoList = new List<VideoData>();
            //A loop to run through the 5 videos.
            for (int i = 0; i < videoTitles.Count; i++)
            {
                //Create a new VideoData object 'video'.
                VideoData video = new VideoData()
                {
                    //Add the video data to the object variables by using the lists created when searching for the data.
                    //'.Text' gets the value of the tag.
                    //'.GetAttribute("...")' gets the value of the given attribute.
                    Title = videoTitles[i].GetAttribute("title"),
                    Uploader = videoUploader[i].Text,
                    Views = videoViews[i].Text,
                    Url = videoUrl[i + 1].GetAttribute("href")
                };
                //Print the set variables to the console to show the video data.
                Console.WriteLine("\nVideo " + (i + 1) + "\nTitle: " + video.Title + "\nUploaded by: " + video.Uploader + "\nViews: " + video.Views + "\nLink: " + video.Url);
                //Add the newly created 'video' object to the 'videoList'.
                videoList.Add(video);
            }

            //I ask if the output should be written to .csv and .json files, press 'y' to confirm, press 'n' to deny.
            Console.WriteLine("\nDo you want to output the result to a .CSV and .JSON file ?\n\nPress y to confirm\nPress n to deny");
            ConsoleKeyInfo wantFiles = Console.ReadKey(intercept: true);
            //If 'y' is pressed, start to convert the output into the files.
            if (wantFiles.KeyChar == 'y')
            {
                //The next 2 lines are used to convert the list into a JSON format.
                //The options are provided to format it in a proper way, including newlines and indents.
                var options = new JsonSerializerOptions { WriteIndented = true };
                //Turn the videoList into a JSON file by using the options provided above.
                string jsonString = System.Text.Json.JsonSerializer.Serialize(videoList, options);
                //Convert the videoList into a CSV file.
                string csv = CsvSerializer.SerializeToCsv(videoList);
                //Write both created files 'jsonString' and 'csv' to files. These files will be located in the root of the project.
                //bin/debug/net6.0/
                File.WriteAllText("videos.json", jsonString);
                File.WriteAllText("videos.csv", csv);
            }      
        }
    }
}
