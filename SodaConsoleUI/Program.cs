using SodaConsoleUI;

Startup.ConfigureApp();
var serviceProvider = Startup.RegisterServices();

Menu menu = new Menu(serviceProvider);
menu.Show();
