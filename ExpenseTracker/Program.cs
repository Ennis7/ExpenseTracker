using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                Console.WriteLine("3. Current Average Expense");
                Console.WriteLine("4. Budget Calculator");
                Console.WriteLine("5. Instructions");
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
                        DisplayInstructions();
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

        private static void DisplayInstructions()
        {
            DisplayHeader("Instructions");

            Console.WriteLine("This application is design to help the user track their expenses and keep a budget. The main menu consist of...\n\n" +
                "Input Expenses:\n\n This is where daily expenses are enter (example: amount, description, date) when finished the user will enter done and then push any button to return to the main menu.\n\n" +
                "Review Expenses:\n\n When reviewing expenses the user will see the expenses listed under the date they were entered for. Review expenses has it's own menu at the bottom of the screen, (edit = 1, delete = 2, and exit = E). These menu opinions can be used to edit an already exsisting expenses or to delete one, exit is used to return to the main menu.\n\n" +
                "Expense Average:\n\n Gives you the average expense of all entrys entered.\n\n" +
                "Budget Calculator:\n\n Has another menu (set budget = 1, review budget = 2, and exit = E) this is where the user will set a daily budget for themselves, review their daily budget against the expenses and see the remaining amount and the percentage of the budget used for each day.");

                DisplayContinuePrompt();
        }

        static void BudgetCalculator()
        {
            string menuChoice;
            bool exiting = false;

            while (!exiting)
            {
                DisplayHeader("Budget Calculator");
                Console.WriteLine("1. Set Budget");
                Console.WriteLine("2. Review Budget");
                Console.WriteLine("E. Exit");
                Console.WriteLine("Enter Choice:");
                menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        SetBudget();
                        break;
                    case "2":
                        ReviewBudget();
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

        static void ReviewBudget()
        {
            // Uses multiple data files in this method

            string dataPath = "Data/Expenses.txt";
            string budgetPath = "Data/Budget.txt";
            bool noBudget = false;

            double budget = 0;

            try
            {
                string[] budgetArray = File.ReadAllLines(budgetPath);
                double.TryParse(budgetArray[0], out budget);
            } catch
            {
                noBudget = true;
            }

            string[] data = File.ReadAllLines(dataPath);
            

            DisplayHeader("Review Budget");

            if(noBudget)
            {
                Console.WriteLine("There doesn't appear to be a budget set, please set a budget before reviewing.");
                DisplayContinuePrompt();
            } else
            {
                Dictionary<string, List<string>> dict = data.GroupBy(s => s.Split(',')[2].Split(' ')[1]).ToDictionary(g => g.Key, g => g.ToList());
                SortedDictionary<string, List<string>> sorted = new SortedDictionary<string, List<string>>(dict);

                foreach (KeyValuePair<string, List<string>> entry in sorted)
                {
                    double dailyTotal = 0;
                    Console.WriteLine(entry.Key);
                    Console.WriteLine();
                    foreach (string expense in entry.Value)
                    {
                        string[] parts = new string[3];
                        parts = expense.Split(',');
                        dailyTotal += Convert.ToDouble(parts[0].Replace("$", ""));
                    }
                    Console.WriteLine($"Total spent: ${dailyTotal}");
                    Console.WriteLine($"Budget remaining: ${Math.Round(budget - dailyTotal, 2)}");
                    Console.WriteLine($"Budget percentage used: %{Math.Round(dailyTotal / budget * 100, 2)}");
                    Console.WriteLine();

                }
                DisplayContinuePrompt();
            }
        }

        static void SetBudget()
        {
            string dataPath = "Data/Budget.txt";
            double budget;
            string menuChoice;
            bool exiting = false;

            DisplayHeader("Set Budget");

            try
            {
                string[] data = File.ReadAllLines(dataPath);
                double.TryParse(data[0], out budget);

                Console.WriteLine($"The current daily budget is ${budget}. Would you like to change it? [yes/no]");
                menuChoice = Console.ReadLine();
                while(!exiting)
                {
                    switch (menuChoice.ToLower())
                    {
                        case "yes":
                            Console.WriteLine("Please enter an amount for your daily budget: [number limit 2 decimals]");
                            while (!double.TryParse(Console.ReadLine(), out budget))
                            {
                                Console.WriteLine("That doesn't appear to be a valid budget, please try again...");
                            }

                            using (var sw = new StreamWriter(dataPath))
                            {
                                sw.WriteLine(Math.Round(budget, 2));
                            }

                            Console.WriteLine($"Budget is now set to ${budget}");
                            exiting = true;
                            break;
                        case "no":
                            exiting = true;
                            break;
                        default:
                            Console.WriteLine("Please make a valid choice.");
                            DisplayContinuePrompt();
                            break;
                    }
                }
                

            } catch
            {
                Console.WriteLine("Please enter an amount for your daily budget: [number limit 2 decimals]");
                while (!double.TryParse(Console.ReadLine(), out budget))
                {
                    Console.WriteLine("That doesn't appear to be a valid budget, please try again...");
                }

                using (var sw = new StreamWriter(dataPath))
                {
                    sw.WriteLine(Math.Round(budget, 2));
                }
                Console.WriteLine($"Budget is now set to ${budget}");
            }
            DisplayContinuePrompt();
        }

        static void AverageExpenses()
        {
            string dataPath = "Data/Expenses.txt";
            List<double> costs = new List<double>();
            double cost;

            DisplayHeader("Expenses Average");

            string[] data = File.ReadAllLines(dataPath);
            foreach (string item in data)
            {
                string[] parts = item.Split(',');
                double.TryParse(parts[0].Split('$')[1], out cost);
                costs.Add(cost);
            }

            double[] costArray = costs.ToArray();

            Console.WriteLine($"Your current expense average is ${Math.Round(costArray.Average(), 2)}");

            DisplayContinuePrompt();
        }

        static void ReviewExpenses()
        {
            string dataPath = "Data/Expenses.txt";
            string menuChoice;
            bool exiting = false;

            while(!exiting)
            {
                DisplayHeader("Review Expenses");

                // reads from file values separated by commas

                string[] data = File.ReadAllLines(dataPath);
                Dictionary<string, List<string>> dict = data.GroupBy(s => s.Split(',')[2].Split(' ')[1]).ToDictionary(g => g.Key, g => g.ToList());
                SortedDictionary<string, List<string>> sorted = new SortedDictionary<string, List<string>>(dict);
                Console.WriteLine(DictionaryToJson(sorted));
                Console.WriteLine();

                Console.WriteLine("1. Edit");
                Console.WriteLine("2. Delete");
                Console.WriteLine("E. Exit");
                Console.WriteLine("Enter Choice:");
                menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        EditExpense(data, sorted);
                        break;
                    case "2":
                        DeleteExpense(data, sorted);
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

        static void DeleteExpense(string[] data, SortedDictionary<string, List<string>> sorted)
        {
            string dateToEdit, confirmDelete;
            bool exiting = false;
            int expenseToEdit;
            string dataPath = "Data/Expenses.txt";


            DisplayHeader("Edit Expense");

            Console.WriteLine("For which date would you like delete an expense? [mm/dd/yy]");
            Console.WriteLine("Available dates are: ");
            foreach (KeyValuePair<string, List<string>> entry in sorted)
            {
                Console.WriteLine(entry.Key);
            }
            Console.WriteLine();
            dateToEdit = Console.ReadLine();
            while (!sorted.ContainsKey(dateToEdit))
            {
                Console.WriteLine("That doesn't appear to be a valid date, please check the format and try again...");
                dateToEdit = Console.ReadLine();
            }
            Console.WriteLine("Which expense would you like to delete? [numeric value]");
            for (int i = 0; i < sorted[dateToEdit].Count; i++)
            {
                Console.WriteLine($"{i + 1}: {sorted[dateToEdit][i]}");
            }
            Console.WriteLine();

            while(!int.TryParse(Console.ReadLine(), out expenseToEdit) || expenseToEdit - 1 > sorted[dateToEdit].Count - 1)
            {
                Console.WriteLine("That doesn't seem to be a valid number. Please try again...");
                Console.WriteLine(sorted[dateToEdit].Count);
            }

            Console.WriteLine($"The current entry is [{sorted[dateToEdit][expenseToEdit - 1]}] are you sure you'd like to delete it? [yes/no]");
            while (!exiting)
            {
                confirmDelete = Console.ReadLine();

                if (confirmDelete.ToLower() == "yes")
                {
                    data = data.Where(val => val != sorted[dateToEdit][expenseToEdit - 1]).ToArray();
                    File.WriteAllLines(dataPath, data);
                    exiting = true;
                }
                else if (confirmDelete.ToLower() == "no")
                {
                    exiting = true;
                }
                else
                {
                    Console.WriteLine("Unknown response, please answer yes or no...");
                }
            }
        }

        static void EditExpense(string[] data, SortedDictionary<string, List<string>> sorted)
        {
            string dateToEdit, changedExpense;
            bool exiting = false;
            int expenseToEdit, index;
            string dataPath = "Data/Expenses.txt";


            DisplayHeader("Edit Expense");

            Console.WriteLine("For which date would you like edit an expense? [mm/dd/yy]");
            Console.WriteLine("Available dates are: ");
            foreach (KeyValuePair<string, List<string>> entry in sorted)
            {
                Console.WriteLine(entry.Key);
            }
            Console.WriteLine();
            dateToEdit = Console.ReadLine();
            while (!sorted.ContainsKey(dateToEdit))
            {
                Console.WriteLine("That doesn't appear to be a valid date, please check the format and try again...");
                dateToEdit = Console.ReadLine();
            }

            Console.WriteLine("Which expense would you like to edit? [numeric value]");
            for(int i = 0; i < sorted[dateToEdit].Count; i++)
            {
                Console.WriteLine($"{i + 1}: {sorted[dateToEdit][i]}");
            }
            Console.WriteLine();
            while (!int.TryParse(Console.ReadLine(), out expenseToEdit) || expenseToEdit - 1 > sorted[dateToEdit].Count - 1)
            {
                Console.WriteLine("That doesn't seem to be a valid number. Please try again...");
                Console.WriteLine(sorted[dateToEdit].Count);
            }

            Console.WriteLine($"The current entry is [{sorted[dateToEdit][expenseToEdit - 1]}] what would you like to change it to? Type cancel to exit without changes.");
            while(!exiting)
            {
                changedExpense = Console.ReadLine();

                if (ValidateExpense(changedExpense) && changedExpense.ToLower() != "cancel")
                {
                    index = Array.IndexOf(data, sorted[dateToEdit][expenseToEdit - 1]);
                    data[index] = changedExpense;
                    File.WriteAllLines(dataPath, data);
                    exiting = true;
                }
                else if (changedExpense.ToLower() == "cancel")
                {
                    exiting = true;
                }
                else
                {
                    Console.WriteLine("Unknown expense format. Amount must have a dollar sign and date must follow mm/dd/yy format. Please check and try again... ");
                }
            }
        }

        static void InputExpenses()
        {
            string expense;
            bool exiting = false;

            string dataPath = "Data/Expenses.txt";

            DisplayHeader("Input Expenses");
            Console.WriteLine("Enter any new expenses in this format [$1.00, description, mm/dd/yy] then press enter, type done when finished:");
            while (!exiting)
            {
                expense = Console.ReadLine();

                if (ValidateExpense(expense) && expense.ToLower() != "done")
                {
                    // writes to file values separated by commas

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
                    Console.WriteLine("Unknown expense format. Amount must have a dollar sign and date must follow mm/dd/yy format. Please check and try again... ");
                }
            }

            DisplayContinuePrompt();
        }

        // validate expense returns a boolean value

        static bool ValidateExpense(string expense)
        {
            string[] parts = new string[3];

            // regex to check string formatting and limit decimals in currency

            Regex costCheck = new Regex(@"^[\$][0-9]*(\.[0-9]{1,2})?$");
            Regex dateCheck = new Regex(@"^ (0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.]\d\d$");

            // split data on commas for validating each section before writing to file

            parts = expense.Split(',');

            if (costCheck.IsMatch(parts[0]) && dateCheck.IsMatch(parts[2]))
            {
                return true;
            }
            else
            {
                return false;
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

        // returns formatted dictionary for printing to the console

        static string DictionaryToJson(SortedDictionary<string, List<string>> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\n{0}: \n\n{1}", d.Key, string.Join("\n", d.Value)));
            return string.Join("\n", entries);
        }
    }
}
