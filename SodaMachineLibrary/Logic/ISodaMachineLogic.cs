using SodaMachineLibrary.Models;

namespace SodaMachineLibrary.Logic;

public interface ISodaMachineLogic
{
    List<SodaModel> ListTypesOfSoda();

    decimal MoneyInserted(string userId, decimal monetaryAmount);

    decimal GetSodaPrice();

    (SodaModel? soda, List<CoinModel>? change, string errorMessage) RequestSoda(SodaModel soda, string userId);

    void IssueFullRefund(string userId);

    decimal GetMoneyInsertedTotal(string userId);

    void AddToSodaInventory(List<SodaModel> sodas);

    List<SodaModel> GetSodaInventory();

    decimal EmptyMoneyFromMachine();

    List<CoinModel> GetCoinInventory();

    void AddToCoinInventory(List<CoinModel> coins);

    decimal GetCurrentIncome();

    decimal GetTotalIncome();
}
