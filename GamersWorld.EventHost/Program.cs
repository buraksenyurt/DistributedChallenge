using GamersWorld.EventHost;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddEventExecutors();

var serviceProvider = services.BuildServiceProvider();
var factory = serviceProvider.GetService<EventExecuterFactory>();

//TODO@buraksenyurt Buraya RabbitMQ implementasyonu gelmeli.

Console.WriteLine("Message listener is ready. Press [enter] to exit.");
Console.ReadLine();