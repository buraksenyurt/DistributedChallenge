using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretsAgent;
using Loki.Requests.Kahin;
using Loki.Model;

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

var targets = new List<Target>
{
    new() {
        Name="Kahin Reporting Gatewaw Api",
        Action="Get Report",
        Uri=new Uri($"http://{await secretService.GetSecretAsync("KahinReportingGatewayApiAddress")}/getReport"),
        //Uri=new Uri($"http://localhost:5218/getReport"),
        Payload=new GetReportRequest
                {
                    DocumentId = $"1001-abc-{Guid.NewGuid()}"
                }
    },
    new() {
        Name="Kahin Reporting Gatewaw Api",
        Action="New Report Request",
        Uri=new Uri($"http://{await secretService.GetSecretAsync("KahinReportingGatewayApiAddress")}/"),
        //Uri=new Uri($"http://localhost:5218/"),
        Payload=new CreateReportRequest
                {
                    TraceId=Guid.NewGuid().ToString(),
                    EmployeeId="BRK-1903",
                    Expression="SELECT * FROM Reports WHERE CategoryId=1 ORDER BY Id Desc",
                    Title="Sales from the last 4 period"
                }
    }
};

int numberOfRequests = args.Length > 0 ? int.Parse(args[0]) : 10000;
int degreeOfParallelism = args.Length > 1 ? int.Parse(args[1]) : 100;

logger.LogInformation(
    "DDoS simulation has been started. Number of Request/Degree Of Parallel...({NumberOfRequests}/{DegreeOfParallelism})"
    , numberOfRequests
    , degreeOfParallelism);

foreach (var target in targets)
{
    logger.LogInformation("Starting DDoS simulation on {Url}", target.Uri);
    await StartDdosAttack(target, numberOfRequests, degreeOfParallelism, logger);
    logger.LogInformation("Simulation completed for {Url}", target.Uri);
}

logger.LogInformation("All simulations completed...");

async Task StartDdosAttack(
    Target service
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
                if (service.HttpMethod == HttpMethod.Post)
                {
                    string json = JsonSerializer.Serialize(service.Payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(service.Uri.ToString(), content);
                        logger.LogInformation("Request {ReqId}: {StatusCode}", reqId, response.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Request {ReqId} failed", reqId);
                    }
                }
            }
        });
    }

    await Task.WhenAll(tasks);
}
