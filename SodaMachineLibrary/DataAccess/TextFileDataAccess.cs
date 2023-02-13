using Microsoft.Extensions.Configuration;
using SodaMachineLibrary.Models;
using System.Drawing;

namespace SodaMachineLibrary.DataAccess;

public class TextFileDataAccess : IDataAccess
{
    private string _coinTextFile;
    private string _sodaTextFile;
    private string _machineInfoTextFile;
    private string _userCreditTextFile;

    public TextFileDataAccess(IConfiguration config)
    {
        _coinTextFile = config.GetValue<string>("TextFileStorage:Coins") ?? "";
        _sodaTextFile = config.GetValue<string>("TextFileStorage:Soda") ?? "";
        _machineInfoTextFile = config.GetValue<string>("TextFileStorage:MachineInfo") ?? "";
        _userCreditTextFile = config.GetValue<string>("TextFileStorage:UserCredit") ?? "";
    }

    private List<CoinModel> RetrieveCoins()
    {
        var output = new List<CoinModel>();

        var lines = File.ReadAllLines(_coinTextFile);

        try
        {
            foreach (string line in lines)
            {
                var coin = line switch
                {
                    "Dollar" => CoinModel.Dollar,
                    "Quarter" => CoinModel.Quarter,
                    "Dime" => CoinModel.Dime,
                    "Nickel" => CoinModel.Nickel,
                    _ => throw new FormatException($"No valid match found for {line}.")
                };
                output.Add(coin);
            }
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new Exception("Missing data in the Coin text file.", ex);
        }
        catch (FormatException ex)
        {
            throw new Exception("The data in the Coin text file has been corrupted.", ex);
        }
        catch
        {
            throw;
        }

        return output;
    }
    private void SaveCoins(List<CoinModel> coins)
    {
        File.WriteAllLines(_coinTextFile, coins.Select(c => c.Name));
    }
    private (decimal sodaPrice, decimal cashOnHand, decimal totalIncome) RetrieveMachineInfo()
    {
        (decimal sodaPrice, decimal cashOnHand, decimal totalIncome) output = (0, 0, 0);

        var lines = File.ReadAllText(_machineInfoTextFile).Split(",");

        try
        {
            output.sodaPrice = decimal.Parse(lines[0]);
            output.cashOnHand = decimal.Parse(lines[1]);
            output.totalIncome = decimal.Parse(lines[2]);
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new Exception("Missing data in the Machine Info text file.", ex);
        }
        catch (FormatException ex)
        {
            throw new Exception("The data in the Machine Info text file has been corrupted.", ex);
        }
        catch
        {
            throw;
        }

        return output;
    }
    private void SaveMachineInfo((decimal, decimal, decimal) machineInfo)
    {
        File.WriteAllText(_machineInfoTextFile, machineInfo.ToString().Remove('(').Remove(')'));
    }
    private List<SodaModel> RetrieveSodas()
    {
        var output = new List<SodaModel>();

        var lines = File.ReadAllLines(_sodaTextFile);

        try
        {
            foreach (string line in lines)
            {
                var data = line.Split(',');
                output.Add(new SodaModel(data[0], data[1]));
            }
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new Exception("Missing data in the Coin text file.", ex);
        }
        catch
        {
            throw;
        }

        return output;

    }
    private void SaveSodas(List<SodaModel> sodas)
    {
        File.WriteAllLines(_sodaTextFile, sodas.Select(s => $"{s.Name},{s.SlotOccupied}"));
    }
    private Dictionary<string, decimal> RetrieveUserCredit()
    {
        var output = new Dictionary<string, decimal>();

        var lines = File.ReadAllLines(_userCreditTextFile);

        try
        {
            foreach (string line in lines)
            {
                var data = line.Split(',');
                output.Add(data[0], decimal.Parse(data[1]));
            }
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new Exception("Missing data in the User Credit text file.", ex);
        }
        catch (FormatException ex)
        {
            throw new Exception("Corrupted data in the User Credit text file.", ex);
        }
        catch
        {
            throw;
        }

        return output;
    }
    private void SaveUserCredit(Dictionary<string, decimal> userCredit)
    {
        File.WriteAllLines(_userCreditTextFile, userCredit.Select(x => $"{x.Key},{x.Value}"));
    }

    public void CoinInventory_AddCoins(List<CoinModel> coins)
    {
        coins.AddRange(RetrieveCoins());
        SaveCoins(coins);
    }

    public List<CoinModel> CoinInventory_GetAll()
    {
        return RetrieveCoins();
    }

    public List<CoinModel> CoinInventory_WithdrawCoins(CoinModel coinType, int quantity)
    {
        var coins = RetrieveCoins();
        var coinsWithdrawn = coins.Where(c => c.Name == coinType.Name).Take(quantity).ToList();
        coinsWithdrawn.ForEach(c => coins.Remove(c));
        SaveCoins(coins);
        return coinsWithdrawn;
    }

    public decimal MachineInfo_CashOnHand()
    {
        return RetrieveMachineInfo().cashOnHand;
    }

    public decimal MachineInfo_EmptyCash()
    {
        var info = RetrieveMachineInfo();
        var cash = info.cashOnHand;
        info.cashOnHand = 0m;
        SaveMachineInfo(info);
        return cash;

    }

    public decimal MachineInfo_SodaPrice()
    {
        return RetrieveMachineInfo().sodaPrice;
    }

    public decimal MachineInfo_TotalIncome()
    {
        return RetrieveMachineInfo().totalIncome;
    }

    public void SodaInventory_AddSodas(List<SodaModel> sodas)
    {
        sodas.AddRange(RetrieveSodas());
        SaveSodas(sodas);
    }

    public bool SodaInventory_CheckIfSodaInStock(SodaModel soda)
    {
        return RetrieveSodas().Any(s => s.Name == soda.Name && s.SlotOccupied == soda.SlotOccupied);
    }

    public List<SodaModel> SodaInventory_GetAll()
    {
        return RetrieveSodas();
    }

    public SodaModel SodaInventory_GetSoda(SodaModel soda)
    {
        return RetrieveSodas()
                .Where(s => s.SlotOccupied == soda.SlotOccupied && s.Name == soda.Name)
                .FirstOrDefault()!;
    }

    public List<SodaModel> SodaInventory_GetTypes()
    {
        return RetrieveSodas()
                .GroupBy(s => s.Name)
                .Select(s => s.First())
                .ToList();
    }

    public void UserCredit_Clear(string userId)
    {
        var info = RetrieveUserCredit();

        if (info.ContainsKey(userId))
        {
            info.Remove(userId);
        }

        SaveUserCredit(info);
    }

    public void UserCredit_Deposit(string userId)
    {
        var uc = RetrieveUserCredit();

        if (!uc.ContainsKey(userId)) throw new Exception("User not found");

        var info = RetrieveMachineInfo();
        info.cashOnHand += uc[userId];
        info.totalIncome += uc[userId];
        SaveMachineInfo(info);

        uc.Remove(userId);
        SaveUserCredit(uc);
    }

    public void UserCredit_Insert(string userId, decimal amount)
    {
        var uc = RetrieveUserCredit();
        uc.TryGetValue(userId, out decimal currentCredit);
        uc[userId] = currentCredit + amount;
        SaveUserCredit(uc);
    }

    public decimal UserCredit_Total(string userId)
    {
        RetrieveUserCredit().TryGetValue(userId, out decimal currentCredit);
        return currentCredit;
    }

    public void UserCredit_Withdraw(string userId, decimal amount)
    {
        var uc = RetrieveUserCredit();
        
        if (!uc.TryGetValue(userId, out decimal currentCredit)) throw new Exception("User not found");
        if (currentCredit < amount) throw new Exception("Insufficient funds");

        uc[userId] -= amount;
        SaveUserCredit(uc);
    }
}