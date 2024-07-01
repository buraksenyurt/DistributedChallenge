using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretsAgent;
using Loki.Requests.Kahin;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfiguration configuration = builder.Build();

var serviceProvider = new ServiceCollection()
    .AddLogging(configure => configure.AddConsole())
    .AddSingleton(configuration)
    .AddSingleton<ISecretStoreService, SecretStoreService>()
    .BuildServiceProvider();

var logger = serviceProvider.GetService<ILogger<Program>>();
var secretService = serviceProvider.GetService<ISecretStoreService>();

var urls = new List<string>
{
    $"http://{await secretService.GetSecretAsync("KahinReportingGatewayApiAddress")}/getReport",
    //"http://localhost:5228/",
};

int numberOfRequests = args.Length > 0 ? int.Parse(args[0]) : 10000;
int degreeOfParallelism = args.Length > 1 ? int.Parse(args[1]) : 100;

logger.LogInformation(
    "DDoS simulation has been started. Number of Request/Degree Of Parallel...({NumberOfRequests}/{DegreeOfParallelism})"
    , numberOfRequests
    , degreeOfParallelism);

foreach (var url in urls)
{
    logger.LogInformation("Starting DDoS simulation on {Url}", url);
    await StartDdosAttack(url, numberOfRequests, degreeOfParallelism, logger);
    logger.LogInformation("Simulation completed for {Url}", url);
}

logger.LogInformation("All simulations completed...");

async Task StartDdosAttack(
    string url
    , int numberOfRequests
    , int degreeOfParallelism
    , ILogger logger)
{
    var client = new HttpClient();
    var tasks = new Task[degreeOfParallelism];

    for (int coreId = 0; coreId < degreeOfParallelism; coreId++)
    {
        tasks[coreId] = Task.Run(async () =>
        {
            for (int reqId = 0; reqId < numberOfRequests / degreeOfParallelism; reqId++)
            {
                var request = new GetReportRequest
                {
                    DocumentId = $"1001-abc-{Guid.NewGuid()}"
                };
                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(url, content);
                    logger.LogInformation("Request {ReqId}: {StatusCode}", reqId, response.StatusCode);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Request {ReqId} failed", reqId);
                }
            }
        });
    }

    await Task.WhenAll(tasks);
}
