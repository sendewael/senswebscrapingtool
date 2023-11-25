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
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;


namespace Webscraping
{
    //This class is created to be able to create 'AhData' objects to put into a list later on.
    public class AhData
    {
        //A AhData object takes the following arguments and stores them in the object.
        public string Name { get; set; }
        public string Price { get; set; }
        public string Bonus { get; set; }
        public string UnitSize { get; set; }
    }

    public class AhBonusScraping
    {
        public static void AhBonusScrape()
        {
            //I ask the user for input. The input is written to the 'searchTermBonus' variable.
            Console.WriteLine("Check for a bonus product:");
            //UNCOMMENT FOR NORMAL
            string searchTermBonus = Console.ReadLine();

            //UNCOMMENT FOR AH
            //This line can be uncommented to set a default product.
            //string searchTermBonus = "skyr";

            Console.WriteLine("\nSome Geckodriver stuff...");

            //This line of code starts the Firefox driver, and opens the browser.
            IWebDriver driver = new FirefoxDriver();

            //The browser opens the given url. I pass the 'searchTermBonus' variable.
            driver.Url = "https://www.ah.be/zoeken?query=" + searchTermBonus;

            //Implicit wait is used to wait until all elements I search for are loaded, it will throw an error after 5 sec if not all elements are found.
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            
            //This selects the ignore cookie button and clicks it.
            IWebElement cookieButton = driver.FindElement(By.XPath("//button[@id='decline-cookies']"));
            cookieButton.Click();

            //If I give a product name of a product not available at Albert Heijn, find the element on the page that indicates this and put it into a list.
            //If there is a value in the list, write a line to tell the user that no products are found and exit the app.
            IReadOnlyList<IWebElement> noProduct = driver.FindElements(By.XPath("//div[@data-testhook='search-no-results']"));
            if (noProduct.Count() > 0)
            {
                Console.WriteLine("\nThere are no bonus products found for the given product.\nPlease close this tool and try a different product.");
                Environment.Exit(0);
            }

            //I search for the 'bonus' button element on the page and put them in a list.
            //It then checks if it found any bonus buttons.
            //If not found, show a message saying there are no bonus products.
            //Else click the bonus button.
            IReadOnlyList<IWebElement> bonusButton = driver.FindElements(By.XPath("//a[@aria-label='filter: Bonus']"));
            if (bonusButton.Count() == 0 )
            {
                Console.WriteLine("There are no bonus products. Please exit the tool and restart to search again.");
                Environment.Exit(0);
            } else
            {
                bonusButton[0].Click();
            }

            //I find the product names, prices, bonusses and unit size by using their XPath. The data of all found products are put into lists.
            List<IWebElement> productName = driver.FindElements(By.XPath("//strong[@data-testhook='product-title']")).ToList();
            List<IWebElement> productPrice = driver.FindElements(By.XPath("//div[@data-testhook='price-amount']")).ToList();
            List<IWebElement> productBonus = driver.FindElements(By.XPath("//div[@data-testhook='product-shield']")).ToList();
            List<IWebElement> productUnitSize = driver.FindElements(By.XPath("//span[@data-testhook='product-unit-size']")).ToList();

            //Just some text to make it prettier.
            Console.WriteLine("\nThese bonus products are found for '" + searchTermBonus + "' on Albert Heijn:");

            //I create a new list 'ahList' that takes objects of type 'AhData'. This list will contain all products with their corresponding data.
            //I create this list to be able to convert it to .JSON and .CSV in a proper and easy way.
            List<AhData> ahList = new List<AhData>();
            //A loop to run through all products.
            for (int i = 0; i < productName.Count; i++)
            {
                //Create a new AhData object 'ah'.
                AhData ah = new AhData()
                {
                    //Add the product data to the object variables by using the lists created when searching for the data.
                    //'.Text' gets the value of the tag.
                    Name = productName[i].Text,
                    Price = productPrice[i].Text,
                    Bonus = productBonus[i].Text,
                    UnitSize = productUnitSize[i].Text,

                };
                //Print the set variables to the console to show the product data.
                Console.WriteLine("\nProduct " + (i + 1) + "\nProduct name: " + ah.Name + "\nUnit Size: " + ah.UnitSize + "\nPrice: " + ah.Price + " euros" + "\nBonus: " + ah.Bonus);
                //Add the newly created 'ah' object to the 'ahList'.
                ahList.Add(ah);
            }

            //I ask if the output should be written to .csv and .json files, press 'y' to confirm, press 'n' to deny.
            Console.WriteLine("\nDo you want to output the result to a .CSV and .JSON file ?\n\nPress y to confirm\nPress n to deny");
            
            //UNCOMMENT FOR NORMAL
            ConsoleKeyInfo wantFiles = Console.ReadKey(intercept: true);
            
            //UNCOMMENT FOR AH
            //char wantFiles = 'n';
            //If 'y' is pressed, start to convert the output into the files.
            //UNCOMMENT FOR AH
            //if (wantFiles == 'y')
            //UNCOMMENT FOR NORMAL
            if (wantFiles.KeyChar == 'y')
            {
                //The next 2 lines are used to convert the list into a JSON format.
                //The options are provided to format it in a proper way, including newlines and indents.
                var options = new JsonSerializerOptions { WriteIndented = true };
                //Turn the ahList into a JSON file by using the options provided above.
                string jsonString = System.Text.Json.JsonSerializer.Serialize(ahList, options);
                //Convert the ahList into a CSV file.
                string csv = CsvSerializer.SerializeToCsv(ahList);
                //Write both created files 'jsonString' and 'csv' to files. These files will be located in the root of the project.
                //bin/debug/net6.0/
                File.WriteAllText("ahbonus.json", jsonString);
                File.WriteAllText("ahbonus.csv", csv);
            }

            //Here i make a final check to see if there are objects in the 'ahList', if yes = bonus products found.
            //Then run the Pushbullet method which will send a notification on my phone.
            if (ahList.Count > 0)
            {
                Pushbullet();
            }
        }

        //This method sends a notification to your phone if bonus products are found.
        public static void Pushbullet()
        {
            
            //Change the api key to your own api key created on the Pushbullet website.
            var apiKey = "YOUR API KEY HERE";
            //Create client
            PushbulletClient client = new PushbulletClient(apiKey);

            //Get information about the user account behind the API key
            var currentUserInformation = client.CurrentUsersInformation();

            //Check if useraccount data could be retrieved
            if (currentUserInformation != null)
            {
                //Create request
                PushNoteRequest request = new PushNoteRequest
                {
                    Email = currentUserInformation.Email,
                    //This is the message that is sent
                    Title = "Message title",
                    Body = "Message body"
                };

                PushResponse response = client.PushNote(request);
            }
        }
    }
}
