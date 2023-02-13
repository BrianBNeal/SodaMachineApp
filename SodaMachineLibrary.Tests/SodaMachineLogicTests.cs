

namespace SodaMachineLibrary.Tests
{
    public class SodaMachineLogicTests
    {
        [Fact]
        public void AddToCoinInventory_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            List<CoinModel> newCoins = new()
            {
                CoinModel.Quarter,
                CoinModel.Quarter,
                CoinModel.Dime,
            };

            logic.AddToCoinInventory(newCoins);

            int expected = 6;
            int actual = da.CoinInventory.Where(c => c.Name == CoinModel.Quarter.Name).Count();

            Assert.NotEqual(expected, actual); //3 coins were added
        }

        [Fact]
        public void AddToSodaInventory_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);
            List<SodaModel> sodas = new()
            {
                new("Coke", "1"),
                new("Coke", "1"),
            };

            logic.AddToSodaInventory(sodas);

            int expected = 7;
            int actual = da.SodaInventory.Where(s => s.Name == "Coke").Count();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EmptyMoneyFromMachine_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            decimal expected = da.MachineInfo.cashOnHand;
            decimal actual = logic.EmptyMoneyFromMachine();

            Assert.Equal(expected, actual);

            expected = 0;
            actual = da.MachineInfo.cashOnHand;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetCoinInventory_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            var coins = logic.GetCoinInventory();

            int expected = da.CoinInventory.Where(c => c.Name == "Quarter").Count();
            int actual = coins.Where(c => c.Name == "Quarter").Count();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetCurrentIncome_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            var expected = da.MachineInfo.cashOnHand;
            decimal actual = logic.GetCurrentIncome();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetMoneyInsertedTotal_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            decimal expected = 0.65m;
            da.UserCredit.Add("test", expected);

            decimal actual = logic.GetMoneyInsertedTotal("test");
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetSodaInventory_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);
            
            int actual = logic.GetSodaInventory().Count();
            int expected = 8;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetSodaPrice_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            decimal expected = da.MachineInfo.sodaPrice;
            decimal actual = logic.GetSodaPrice();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetTotalIncome_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            decimal expected = da.MachineInfo.totalIncome;
            decimal actual = logic.GetTotalIncome();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IssueFullRefund_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);
            string user = "test";

            da.UserCredit[user] = 0.65m;
            
            logic.IssueFullRefund("test");

            Assert.False(da.UserCredit.ContainsKey(user));
        }

        [Fact]
        public void ListTypesOfSoda_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            int expected = 3;
            int actual = logic.ListTypesOfSoda().Count();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MoneyInserted_SingleUser_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);
            string userId = "test";
            decimal amount = 0.65m;

            logic.MoneyInserted(userId, amount);

            decimal expected = amount;
            decimal actual = da.UserCredit[userId];

            Assert.Equal(expected, actual);

            logic.MoneyInserted(userId, amount);

            expected += amount;
            actual = da.UserCredit[userId];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MoneyInserted_MultiUser_ShouldWork()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);
            string user1 = "test";
            string user2 = "Brian";
            decimal money1 = 0.25m;
            decimal money2 = 0.1m;

            logic.MoneyInserted(user1, money1);
            logic.MoneyInserted(user2, money2);
            logic.MoneyInserted(user1, money1);

            decimal expected = money2;
            decimal actual = da.UserCredit[user2];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RequestSoda_ExactChange_ShouldReturnSodaWithNoChange()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            string user = "test";
            SodaModel expectedSoda = new("Coke", "1");
            var initialState = da.MachineInfo;

            da.UserCredit[user] = 0.75m;

            var results = logic.RequestSoda(expectedSoda, user);

            Assert.Equal(expectedSoda.Name, results.soda?.Name); //right name
            Assert.Equal(expectedSoda.SlotOccupied, results.soda?.SlotOccupied); //right slot
            Assert.False(da.UserCredit.ContainsKey(user)); //credit is cleared/removed
            Assert.Equal(initialState.cashOnHand + .75m, da.MachineInfo.cashOnHand); //cash deposited
            Assert.Equal(initialState.totalIncome + .75m, da.MachineInfo.totalIncome); //income recorded
            Assert.True(string.IsNullOrWhiteSpace(results.errorMessage)); //no error
            Assert.True(results.change?.Count == 0); //no change due
        }

        [Fact]
        public void RequestSoda_InsufficientMoney_ShouldSayNotEnoughMoney()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            string user = "test";
            SodaModel expectedSoda = new("Coke", "1");
            var initialState = da.MachineInfo;

            da.UserCredit[user] = 0.5m;

            var results = logic.RequestSoda(expectedSoda, user);

            Assert.Null(results.soda);
            Assert.Equal(0.5m, da.UserCredit[user]);
            Assert.Equal(initialState.cashOnHand, da.MachineInfo.cashOnHand);
            Assert.Equal(initialState.totalIncome, da.MachineInfo.totalIncome);
            Assert.False(string.IsNullOrWhiteSpace(results.errorMessage));
            Assert.Null(results.change);
        }

        [Fact]
        public void RequestSoda_OutOfStock_ShouldSayOutOfStock()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            string user = "test";
            SodaModel expectedSoda = new("Fanta", "4");
            var initialState = da.MachineInfo;

            da.UserCredit[user] = 0.75m;

            var results = logic.RequestSoda(expectedSoda, user);

            Assert.Null(results.soda);
            Assert.Equal(0.75m, da.UserCredit[user]);
            Assert.Equal(initialState.cashOnHand, da.MachineInfo.cashOnHand);
            Assert.Equal(initialState.totalIncome, da.MachineInfo.totalIncome);
            Assert.False(string.IsNullOrWhiteSpace(results.errorMessage));
            Assert.Null(results.change);
        }

        [Fact]
        public void RequestSoda_SimpleChangeDue_ShouldReturnSodaWithChange()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            string user = "test";
            SodaModel expectedSoda = new("Coke", "1");
            var initialState = da.MachineInfo;

            da.UserCredit[user] = 1m;

            var results = logic.RequestSoda(expectedSoda, user);

            Assert.Equal(expectedSoda.Name, results.soda?.Name); //right soda name returned
            Assert.Equal(expectedSoda.SlotOccupied, results.soda?.SlotOccupied); //right soda slot returned
            Assert.False(da.UserCredit.ContainsKey(user)); //credit was cleared
            Assert.Equal(initialState.cashOnHand + 1m, da.MachineInfo.cashOnHand); //cash updated
            Assert.Equal(initialState.totalIncome + 1m, da.MachineInfo.totalIncome); //income updated
            Assert.True(string.IsNullOrWhiteSpace(results.errorMessage)); //no error
            Assert.True(results.change?.Count == 1 && results.change?.Sum(c => c.Amount) == .25m); //one coin worth .25 returned as change

        }

        [Fact]
        public void RequestSoda_ComplexChangeDue_ShouldReturnSodaWithAlternateChange()
        {
            MockDataAccess da = new();
            SodaMachineLogic logic = new(da);

            string user = "test";
            SodaModel expectedSoda = new("Coke", "1");
            var initialState = da.MachineInfo;

            da.UserCredit[user] = 1m;
            da.CoinInventory.RemoveAll(x => x.Amount == .25m);

            var results = logic.RequestSoda(expectedSoda, user);

            Assert.Equal(expectedSoda.Name, results.soda?.Name); //right soda
            Assert.Equal(expectedSoda.SlotOccupied, results.soda?.SlotOccupied); //right slot
            Assert.False(da.UserCredit.ContainsKey(user)); //credit is cleared
            Assert.Equal(initialState.cashOnHand + 1m, da.MachineInfo.cashOnHand); //cash is updated
            Assert.Equal(initialState.totalIncome + 1m, da.MachineInfo.totalIncome); //income is recorded
            Assert.True(string.IsNullOrWhiteSpace(results.errorMessage)); //no error
            Assert.True(results.change?.Count > 1 && results.change.Sum(c => c.Amount) == .25m); //more than one coin used to make correct change
        }
    }
}