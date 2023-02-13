using Microsoft.Extensions.DependencyInjection;
using SodaMachineLibrary.Logic;
using SodaMachineLibrary.Models;
using System.Text;

namespace SodaConsoleUI;

internal class Menu
{
    private string _userSelection;
    private static ISodaMachineLogic _sodaMachine;
    private const string _prompt = " >> ";

    internal Menu(IServiceProvider services)
    {
        _userSelection = string.Empty;
        _sodaMachine = services.GetService<ISodaMachineLogic>()!;
    }

    internal void Show()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("Welcome to our Soda Machine.\r\n");
            DisplayOptions();
            ExecuteOption();

        } while (_userSelection != "9");

        Exit();
    }

    private void Exit()
    {
        Console.Clear();
        Console.WriteLine("Thanks, have a nice day!");
        Console.Write("Press return to quit...");
        Console.ReadLine();
    }

    private void ExecuteOption()
    {
        switch (_userSelection)
        {
            case "1":
                ShowPrice();
                break;

            case "2":
                ListSodas();
                break;

            case "3":
                ShowDepositedAmt();
                break;

            case "4":
                HandleDeposit();
                break;

            case "5":
                CancelTransaction();
                break;

            case "6":
                RequestSoda();
                break;

            case "9": //allow to pass through and exit
            default: //do nothing, replay the menu
                break;
        }
    }

    private void RequestSoda()
    {
        throw new NotImplementedException();
    }

    private void CancelTransaction()
    {
        throw new NotImplementedException();
    }

    private void HandleDeposit()
    {

        var coins = CoinModel.TypeList();
        var validInputs = Enumerable.Range(1, coins.Count);
        string input = string.Empty;
        CoinModel coinToInsert;
        var user = GetUser(true);

        Console.Clear();
        Console.WriteLine("Choose a coin to insert:");
        for (int i = 0; i < coins.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {coins[i].Name} - {string.Format("{0:C}", coins[i].Amount)}");
        }
        Console.WriteLine(_prompt);

        while (string.IsNullOrWhiteSpace(input) || (!char.IsDigit(input[0]) && !validInputs.Contains(int.Parse(input))))
        {
            input = Console.ReadKey(true).KeyChar.ToString();
        }

        Console.WriteLine(input);
        var index = int.Parse(input) - 1;
        coinToInsert = coins[index];

        Console.WriteLine($"Depositing {coinToInsert.Name}...");
        _sodaMachine.MoneyInserted(user, coinToInsert.Amount);
        Console.WriteLine($"{_sodaMachine.GetMoneyInsertedTotal(user)} total deposited for {user}");

        Console.ReadLine();
    }

    private string GetUser(bool clear = false)
    {
        if (clear) Console.Clear();

        var id = string.Empty;

        while (true)
        {
            Console.Write("Enter user ID: ");
            id = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(id)) { break; }

            Console.WriteLine();
        }

        return id;
    }

    private void ShowDepositedAmt()
    {
        Console.Clear();
        Console.WriteLine("*** Show Amount Deposited ***");
        var user = GetUser();
        var amt = _sodaMachine.GetMoneyInsertedTotal(user);
        Console.WriteLine($"Total deposited for {user}: {string.Format("{0:C}", amt)}");

        Console.ReadLine();
    }

    private void ListSodas()
    {
        Console.Clear();
        var sodas = _sodaMachine.ListTypesOfSoda();
        Console.WriteLine("Available Sodas:");
        foreach (var soda in sodas)
        {
            Console.WriteLine($" -{soda.Name}");
        }
        Console.ReadLine();
    }

    private void ShowPrice()
    {
        Console.Clear();
        var price = _sodaMachine.GetSodaPrice();
        Console.WriteLine($"The price for soda is \r\n   {string.Format("{0:C}", price)}.");
        Console.Write("_");
        Console.ReadLine();
    }

    private void DisplayOptions()
    {
        var text = new StringBuilder();

        text.AppendLine("Please make a selection from the following options:");
        text.AppendLine();
        text.AppendLine("1. Show Soda Price");
        text.AppendLine("2. List Soda Options");
        text.AppendLine("3. Show Amount Deposited");
        text.AppendLine("4. Deposit Money");
        text.AppendLine("5. Cancel Transaction");
        text.AppendLine("6. Request Soda");
        text.AppendLine("9. Exit");
        text.AppendLine();
        text.Append(_prompt);

        Console.Write(text);

        _userSelection = Console.ReadKey(true).KeyChar.ToString();
    }
}
