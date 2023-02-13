using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SodaMachineLibrary.DataAccess;
using SodaMachineLibrary.Logic;

namespace SodaConsoleUI;

internal static class Startup
{
    internal static void ConfigureApp()
    {
        Console.Title = "Soda Machine Console";
    }

    internal static IServiceProvider RegisterServices()
    {
        var collection = new ServiceCollection();

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        collection.AddSingleton(config);
        collection.AddTransient<IDataAccess, TextFileDataAccess>();
        collection.AddTransient<ISodaMachineLogic, SodaMachineLogic>();

        return collection.BuildServiceProvider();
    }
}
