//Sources used to create this tool/code:
//https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
//https://www.selenium.dev/documentation/webdriver/
//https://toolsqa.com/selenium-webdriver/c-sharp/webelement-commands-in-c/
//https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
//https://stackoverflow.com/questions/25683161/fastest-way-to-convert-a-list-of-objects-to-csv-with-each-object-values-in-a-new
//https://www.youtube.com/watch?v=VQU58dZEFfo
//https://en.code-bude.net/2018/02/14/send-push-notifications-c/

using OpenQA.Selenium;
using System;
using System.Text;

namespace Webscraping
{
    class MainMenu
    {
        static void Main(string[] args) 
        {
            //A variable 'main' is set to true.
            //The main while loop will continue running until the value of the variable turns into something else.
            string main = "true";
            while (main == "true")
            {
                //This is a character-based welcome message to make things look a bit more interesting.
                Console.WriteLine("/////////////////////////////////////");
                Console.WriteLine("//                                 //");
                Console.WriteLine("//        Thanks for using:        //");
                Console.WriteLine("//     Sen's Webscraping Tool!     //");
                Console.WriteLine("//                                 //");
                Console.WriteLine("/////////////////////////////////////\n");
                Console.WriteLine("Which info would you like to scrape?\n");

                //A variable 'options' is set with the value true.
                //This while loop is created so that if the user gives a non valid option, he will be prompted for a new option.
                //I define it here because I dont want to display the character-based message everytime.
                string options = "true";
                while (options == "true")
                {
                    //These 3 lines give the user an indication of what to press for which option.
                    Console.WriteLine("Press 1 for YouTube Videos on 'youtube.com'");
                    Console.WriteLine("Press 2 for Ict Jobs on 'ictjobs.be'");
                    Console.WriteLine("Press 3 for Albert Heijn Bonusses on 'ah.be'\n");

                    //UNCOMMENT FOR NORMAL
                    //This line asks for user input, and stores it into the 'choice' variable. intercept: true means that the user inputted key is not shown on the console.
                    ConsoleKeyInfo choice = Console.ReadKey(intercept: true);
                    
                    //UNCOMMENT FOR AH
                    //This variable is assigned to be able to automate the tool for my Ah Bonus scraping option. Instead of asking for user input, the option is set to '3'.
                    //char def = '3';
                    //switch (def)
                    //UNCOMMENT FOR NORMAL
                    //Based on the option chosen above, user input or variable defined, respond with the corresponding case.
                    //In those cases the corresponding class with the function is called to start scraping.
                    //After the function is completed, set 'options' to false so that the while loop is exited.
                    switch (choice.KeyChar)
                    {
                        case '1':
                            YouTubeScraping.YouTubeScrape();
                            options = "false";
                            break;

                        case '2':
                            IctJobScraping.IctJobScrape();
                            options = "false";
                            break;

                        case '3':
                            AhBonusScraping.AhBonusScrape();
                            options = "false";
                            break;

                        //If the user presses another key then '1, 2 or 3', the default case is activated.
                        //Show a message that the user has to give a valid option. The 'options' variable is not reassigned in this case, so the while loop keeps running
                        //and asks the user for input again.
                        default:
                            Console.WriteLine("Please choose a valid option\n");
                            break;
                    }
                }
                
                //Another while loop is created to make sure the user gives correct input.
                string ran = "true";
                while (ran == "true")
                {
                    //Give the user the choise to scrape data, or exit the tool.
                    Console.WriteLine("\nPress r to scrape data");
                    Console.WriteLine("Press q to exit the tool");
                    ConsoleKeyInfo choose = Console.ReadKey(intercept: true);
                    //UNCOMMENT FOR AH
                    //If my automated AH scraping is used, define the choice and always exit after scraping.
                    //char choose = 'q';
                    //switch (choose)
                    //UNCOMMENT FOR NORMAL
                    switch (choose.KeyChar)
                    {
                        //Clear the console if the user wants to scrape more data. Just to keep things clean.
                        //Set the ran variable to false, so that this loop is exited. And set 'options' to true again so that the loop above starts, to start over.
                        case 'r':
                            Console.Clear();
                            ran = "false";
                            options = "true";
                            break;

                        //Set 'main' and 'ran' to false so that all loops are exited.
                        case 'q':
                            main = "false";
                            ran = "false";
                            Console.WriteLine("\nThanks for using Sen's Webscraping Tool!\nYou may close this window..");
                            break;

                        default:
                            Console.WriteLine("\nPlease choose a valid option");
                            break;
                    }
                }
            }
        }
    }
}
