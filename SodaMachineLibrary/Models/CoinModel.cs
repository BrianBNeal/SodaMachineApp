using System.Reflection;

namespace SodaMachineLibrary.Models;

public class CoinModel
{
    private CoinModel(decimal amount, string name)
    {
        Amount = amount;
        Name = name;
    }
    public static CoinModel Dollar => new CoinModel(1m, "Dollar");
    public static CoinModel Quarter => new CoinModel(.25m, "Quarter");
    public static CoinModel Dime => new CoinModel(.1m, "Dime");
    public static CoinModel Nickel => new CoinModel(.05m, "Nickel");

    public string Name { get; }
    public decimal Amount { get; }

    public static List<CoinModel> TypeList()
    {
        return typeof(CoinModel)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(CoinModel))
                .Select(p => (CoinModel)p.GetValue(null,null)!)
                .OrderByDescending(c => c.Amount)
                .ToList();
    }
}
