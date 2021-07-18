using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ExpenseTracker
{
    class Program
    {
        //*************************************************

        // Title: Expense Tracker
        // Application Type: Console
        // Description: An application to track expenses
        // Author: Sarah Ennis
        // Date Created: 07/15/2021
        // Last Modified:

        //*************************************************

        static void Main(string[] args)
        {
            DisplayWelcomeScreen("Expense Tracker");
            DisplayMenu();
            DisplayClosingScreen();
        }

        static void DisplayMenu()
        {
            string menuChoice;
            bool exiting = false;

            while (!exiting)
            {
                DisplayHeader("Main Menu");
                Console.WriteLine("1. Input Expenses");
                Console.WriteLine("2. Review Expenses");
                Console.WriteLine("3. Average Expenses");
                Console.WriteLine("4. Budget Calculator");
                Console.WriteLine("5. ");
                Console.WriteLine("6. ");
                Console.WriteLine("7. ");
                Console.WriteLine("E. Exit");
                Console.WriteLine("Enter Choice:");
                menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        InputExpenses();
                        break;
                    case "2":
                        ReviewExpenses();
                        break;
                    case "3":
                        AverageExpenses();
                        break;
                    case "4":
                        BudgetCalculator();
                        break;
                    case "5":

                        break;
                    case "6":

                        break;
                    case "7":

                        break;
                    case "E":
                    case "e":

                        exiting = true;
                        break;
                    default:
                        Console.WriteLine("Please make a valid choice.");
                        DisplayContinuePrompt();
                        break;
                }
            }
        }

        static void BudgetCalculator()
        {
            throw new NotImplementedException();
        }

        static void AverageExpenses()
        {
            throw new NotImplementedException();
        }

        static void ReviewExpenses()
        {
            string dataPath = @"Data/Expenses.txt";

            DisplayHeader("Review Expenses");

            string[] readText = File.ReadAllLines(dataPath);
            foreach (string s in readText)
            {
                Console.WriteLine(s);
            }

            DisplayContinuePrompt();
        }

        static void InputExpenses()
        {
            Regex costCheck = new Regex(@"^[\$][0-9]*(\.[0-9]{1,2})?$");
            Regex dateCheck = new Regex(@"^ (0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.]\d\d$");
            string expense;
            bool exiting = false;
            string[] parts = new string[3];

            string dataPath = @"Data/Expenses.txt";

            DisplayHeader("Input Expenses");
            Console.WriteLine("Enter any new expenses in this format [$1.00, description, mm/dd/yy] then press enter, type done when finished:");
            while (!exiting)
            {
                expense = Console.ReadLine();
                parts = expense.Split(',');

                if (costCheck.IsMatch(parts[0]) && dateCheck.IsMatch(parts[2]) && expense.ToLower() != "done")
                {
                    using (StreamWriter sw = File.AppendText(dataPath))
                    {
                        sw.WriteLine(expense);
                    }
                }
                else if (expense.ToLower() == "done")
                {
                    exiting = true;
                }
                else
                {
                    Console.WriteLine("Unknown expense format, please check and try again...");
                }
            }


        }

        static void DisplayHeader(string headText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(headText);
            Console.WriteLine();
        }

        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static void DisplayWelcomeScreen(string text)
        {
            Console.Clear();
            Console.WriteLine(text);
            DisplayContinuePrompt();
        }

        static void DisplayClosingScreen()
        {
            Console.Clear();
            Console.WriteLine("Thank you, goodbye.");
            DisplayContinuePrompt();
        }
    }
}
