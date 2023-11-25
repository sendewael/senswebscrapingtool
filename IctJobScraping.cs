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
using System.Diagnostics;

namespace Webscraping
{
    //This class is created to be able to create 'JobData' objects to put into a list later on.
    public class JobData
    {
        //A JobData object takes the following arguments and stores them in the object.
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Keywords { get; set; }
        public string Url { get; set; }
    }

    public class IctJobScraping
    {
        public static void IctJobScrape()
        {
            //I ask the user for input. The input is written to the 'searchTermICT' variable.
            Console.WriteLine("Search for jobs:");
            string searchTermICT = Console.ReadLine();

            Console.WriteLine("\nSome Geckodriver stuff...");

            //This line of code starts the Firefox driver, and opens the browser.
            IWebDriver driver = new FirefoxDriver();

            //The browser opens the given url. I pass the 'searchTermICT' variable.
            driver.Url = "https://www.ictjob.be/nl/it-vacatures-zoeken?keywords=" + searchTermICT;

            //Implicit wait is used to wait until all elements I search for are loaded, it will throw an error after 10 sec if not all elements are found.
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //If no jobs are found for the given term, find the element on the page that indicates the amount of jobs and put it into a list.
            //Get the list item, and show the 'Text' or value of it. If the value is '0', this means that no jobs are found. Return a message.
            IReadOnlyList<IWebElement> noJob = driver.FindElements(By.XPath("//div[@class='search-result-header-number-results']//span"));
            string jobAmount = noJob[0].Text;
            if (jobAmount == "0")
            {
                Console.WriteLine("\nThere are no jobs found that suit your search.\nPlease restart the tool and try a different search term.");
                Environment.Exit(0);
            }

            //The button is clicked to show the most recent jobs.
            //driver.FindElement(By.XPath("//*[@id=\"sort-by-date\"]")).Click();

            //I find the job titles, company, location, keywords and URL by using their XPath. The data of the first 5 jobs are put into lists.
            List<IWebElement> jobTitles = driver.FindElements(By.XPath("//h2[@class='job-title']")).Take(5).ToList();
            List<IWebElement> jobCompanies = driver.FindElements(By.XPath("//span[@class='job-company']")).Take(5).ToList();
            List<IWebElement> jobLocations = driver.FindElements(By.XPath("//span[@itemprop='addressLocality']")).Take(5).ToList();
            List<IWebElement> jobKeywords = driver.FindElements(By.XPath("//span[@class='job-keywords']")).Take(5).ToList();
            List<IWebElement> jobUrl = driver.FindElements(By.XPath("//a[@class='job-title search-item-link']")).Take(5).ToList();

            //Just some text to make it prettier.
            Console.WriteLine("\nThese jobs are found for '" + searchTermICT + "' on ictjobs.be:");

            //I create a new list 'jobList' that takes objects of type 'JobData'. This list will contain the 5 jobs with their corresponding data.
            //I create this list to be able to convert it to .JSON and .CSV in a proper and easy way.
            List<JobData> jobList = new List<JobData>();
            //A loop to run through the 5 jobs.
            for (int i = 0; i < jobTitles.Count; i++)
            {
                //Create a new JobData object 'job'.
                JobData job = new JobData()
                {
                    //Add the job data to the object variables by using the lists created when searching for the data.
                    //'.Text' gets the value of the tag.
                    Title = jobTitles[i].Text,
                    Company = jobCompanies[i].Text,
                    Location = jobLocations[i].Text,
                    Keywords = jobKeywords[i].Text,
                    //'.GetAttribute("href")' gets the value of the href attribute.
                    Url = jobUrl[i].GetAttribute("href")
                };
                //Print the set variables to the console to show the job data.
                Console.WriteLine("\nJob " + (i+1) +"\nTitle: " + job.Title + "\nCompany: " + job.Company + "\nLocation: " + job.Location + "\nKeywords: " + job.Keywords + "\nLink to details: " + job.Url);
                //Add the newly created 'job' object to the 'jobList'.
                jobList.Add(job);
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
                //Turn the jobList into a JSON file by using the options provided above.
                string jsonString = System.Text.Json.JsonSerializer.Serialize(jobList, options);
                //Convert the jobList into a CSV file.
                string csv = CsvSerializer.SerializeToCsv(jobList);
                //Write both created files 'jsonString' and 'csv' to files. These files will be located in the root of the project.
                //bin/debug/net6.0/
                File.WriteAllText("jobs.json", jsonString);
                File.WriteAllText("jobs.csv", csv);
            } 
        }
    }
}
