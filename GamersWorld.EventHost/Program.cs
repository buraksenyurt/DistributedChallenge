using GamersWorld.EventHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSettings.json", reloadOnChange: true, optional: false)
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);

services.AddEventDrivers();
services.AddRabbitMq(configuration);
services.AddLogging(cfg => cfg.AddConsole());

var serviceProvider = services.BuildServiceProvider();
var eventConsumer = serviceProvider.GetService<EventConsumer>();

eventConsumer.Start();