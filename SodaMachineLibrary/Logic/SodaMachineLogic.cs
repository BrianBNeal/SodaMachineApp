using SodaMachineLibrary.DataAccess;
using SodaMachineLibrary.Models;
using System.Reflection.Metadata.Ecma335;

namespace SodaMachineLibrary.Logic;

public class SodaMachineLogic : ISodaMachineLogic
{
    private readonly IDataAccess _db;

    public SodaMachineLogic(IDataAccess da)
    {
        _db = da;
    }
    public void AddToCoinInventory(List<CoinModel> coins)
    {
        _db.CoinInventory_AddCoins(coins);
    }

    public void AddToSodaInventory(List<SodaModel> sodas)
    {
        _db.SodaInventory_AddSodas(sodas);
    }

    public decimal EmptyMoneyFromMachine()
    {
        return _db.MachineInfo_EmptyCash();
    }

    public List<CoinModel> GetCoinInventory()
    {
        return _db.CoinInventory_GetAll();
    }

    public decimal GetCurrentIncome()
    {
        return _db.MachineInfo_CashOnHand();
    }

    public decimal GetMoneyInsertedTotal(string userId)
    {
        return _db.UserCredit_Total(userId);
    }

    public List<SodaModel> GetSodaInventory()
    {
        return _db.SodaInventory_GetAll();
    }

    public decimal GetSodaPrice()
    {
        return _db.MachineInfo_SodaPrice();
    }

    public decimal GetTotalIncome()
    {
        return _db.MachineInfo_TotalIncome();
    }

    public void IssueFullRefund(string userId)
    {
        _db.UserCredit_Clear(userId);
    }

    public List<SodaModel> ListTypesOfSoda()
    {
        return _db.SodaInventory_GetTypes();
    }

    public decimal MoneyInserted(string userId, decimal monetaryAmount)
    {
        _db.UserCredit_Insert(userId, monetaryAmount);
        return _db.UserCredit_Total(userId);
    }

    public (SodaModel? soda, List<CoinModel>? change, string errorMessage) RequestSoda(SodaModel soda, string userId)
    {
        (SodaModel? soda, List<CoinModel>? change, string errorMessage) output = (null, null, "An unexpected error occurred.");
        decimal userCredit = _db.UserCredit_Total(userId);
        decimal sodaCost = _db.MachineInfo_SodaPrice();

        if (userCredit < sodaCost)
        {
            output.soda = null;
            output.change = null;
            output.errorMessage = "Insufficient Funds";
        }
        else
        {
            output.soda = _db.SodaInventory_GetSoda(soda);

            if (output.soda is null)
            {
                output.errorMessage = "Out of Stock";
                output.change = null;
            }
            else
            {
                _db.UserCredit_Deposit(userId);
                var changeDue = userCredit - sodaCost;
                output.change = GetChange(userId, changeDue);
                output.errorMessage = 
                    output.change.Sum(c => c.Amount) == changeDue 
                    ? "" 
                    : "Sorry, we ran out of change";
            }
        }

        return output;
    }

    private List<CoinModel> GetChange(string userId, decimal changeDue)
    {
        var changeToGive = new List<CoinModel>();
        var amtStillDue = changeDue;
        var types = CoinModel.TypeList();
        foreach (var type in types)
        {
            if (amtStillDue != 0)
            {

                var coinsNeeded = (int)(amtStillDue / type.Amount);
                var coinsAvailable = _db.CoinInventory_GetAll()
                                        .Where(c => c.Name == type.Name)
                                        .Count();

                if (coinsNeeded > 0 && coinsAvailable > 0)
                {
                    var qty = coinsNeeded <= coinsAvailable ? coinsNeeded : coinsAvailable;
                    var coinsDispensed = _db.CoinInventory_WithdrawCoins(type, qty);
                    changeToGive.AddRange(coinsDispensed);
                    amtStillDue -= coinsDispensed.Sum(c => c.Amount);
                }
            }
        };

        return changeToGive;
    }
}
