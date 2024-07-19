using GamersWorld.JobHost;
using GamersWorld.JobHost.Model;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<JobHeader>(context.Configuration.GetSection("JobHeader"));
        services.AddHangfire(config =>
        {
            config.UseMemoryStorage();
        });

        services.AddHangfireServer();
        services.AddTransient<Worker>();
        services.AddSingleton<IRecurringJobManager, RecurringJobManager>();
        services.AddSingleton<IBackgroundJobClient, BackgroundJobClient>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var workerService = host.Services.GetRequiredService<Worker>();
await workerService.ExecuteJobs();

await host.RunAsync();
