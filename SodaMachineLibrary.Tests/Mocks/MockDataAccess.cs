namespace SodaMachineLibrary.Tests.Mocks;

internal class MockDataAccess : IDataAccess
{
    public List<CoinModel> CoinInventory { get; set; } = new();
    public (decimal sodaPrice, decimal cashOnHand, decimal totalIncome) MachineInfo { get; set; }
    public List<SodaModel> SodaInventory { get; set; } = new();
    public Dictionary<string, decimal> UserCredit { get; set; } = new();
    public MockDataAccess()
    {
        CoinInventory.Add(CoinModel.Quarter);
        CoinInventory.Add(CoinModel.Quarter);
        CoinInventory.Add(CoinModel.Quarter);
        CoinInventory.Add(CoinModel.Quarter);

        CoinInventory.Add(CoinModel.Dime);
        CoinInventory.Add(CoinModel.Dime);

        CoinInventory.Add(CoinModel.Nickel);
        CoinInventory.Add(CoinModel.Nickel);
        CoinInventory.Add(CoinModel.Nickel);
        CoinInventory.Add(CoinModel.Nickel);
        CoinInventory.Add(CoinModel.Nickel);

        MachineInfo = (0.75m, 25.65m, 201.50m);

        SodaInventory.Add(new SodaModel("Coke", "1"));
        SodaInventory.Add(new SodaModel("Coke", "1"));
        SodaInventory.Add(new SodaModel("Coke", "1"));
        SodaInventory.Add(new SodaModel("Coke", "1"));
        SodaInventory.Add(new SodaModel("Coke", "1"));

        SodaInventory.Add(new SodaModel("Diet Coke", "2"));

        SodaInventory.Add(new SodaModel("Sprite", "3"));
        SodaInventory.Add(new SodaModel("Sprite", "3"));
    }

    public void CoinInventory_AddCoins(List<CoinModel> coins)
    {
        CoinInventory.AddRange(coins);
    }

    public List<CoinModel> CoinInventory_GetAll()
    {
        return CoinInventory;
    }

    public List<CoinModel> CoinInventory_WithdrawCoins(CoinModel coinType, int quantity)
    {
        var coins = CoinInventory.Where(x => x.Amount == coinType.Amount).Take(quantity).ToList();
        coins.ForEach(c => CoinInventory.Remove(c));
        return coins;
    }

    public decimal MachineInfo_CashOnHand()
    {
        return MachineInfo.cashOnHand;
    }

    public decimal MachineInfo_EmptyCash()
    {
        var val = MachineInfo;
        var output = val.cashOnHand;
        val.cashOnHand = 0M;
        MachineInfo = val;
        return output;
    }

    public decimal MachineInfo_SodaPrice()
    {
        return MachineInfo.sodaPrice;
    }

    public decimal MachineInfo_TotalIncome()
    {
        return MachineInfo.totalIncome;
    }

    public bool SodaInventory_CheckIfSodaInStock(SodaModel soda)
    {
        return SodaInventory.Any(x => x.Name == soda.Name);
    }

    public void SodaInventory_AddSodas(List<SodaModel> sodas)
    {
        SodaInventory.AddRange(sodas);
    }

    public List<SodaModel> SodaInventory_GetAll()
    {
        return SodaInventory;
    }

    public SodaModel SodaInventory_GetSoda(SodaModel soda)
    {
        return SodaInventory
            .Where(x=> x.Name == soda.Name)
            .FirstOrDefault()!;
    }

    public List<SodaModel> SodaInventory_GetTypes()
    {
        return SodaInventory
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .ToList();
    }

    public void UserCredit_Clear(string userId)
    {
        if (UserCredit.ContainsKey(userId))
        {
            UserCredit.Remove(userId);
        }
    }

    public void UserCredit_Deposit(string userId)
    {
        if (!UserCredit.ContainsKey(userId))
        {
            throw new Exception("User not found");
        }

        var info = MachineInfo;
        info.cashOnHand += UserCredit[userId];
        info.totalIncome += UserCredit[userId];
        UserCredit.Remove(userId);

        MachineInfo = info;
    }

    public void UserCredit_Insert(string userId, decimal amount)
    {
        UserCredit.TryGetValue(userId, out decimal currentCredit);
        UserCredit[userId] = currentCredit + amount;
    }

    public decimal UserCredit_Total(string userId)
    {
        UserCredit.TryGetValue(userId, out decimal currentCredit);
        return currentCredit;
    }

    public void UserCredit_Withdraw(string userId, decimal amount)
    {
        if (!UserCredit.TryGetValue(userId, out decimal currentCredit))
        {
            throw new Exception("User not found");
        }

        if (currentCredit < amount)
        {
            throw new Exception("Insufficient funds");
        }

        UserCredit[userId] -= amount;
    }
}
