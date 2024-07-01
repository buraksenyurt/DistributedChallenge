using System.Text;
using System.Text.Json;
using Loki.Requests.Kahin;


//TODO@buraksenyurt Maybe we can use a pattern for teaching different attacks on runtime for specific service

var client = new HttpClient();

var urls = new List<string>
        {
            "http://localhost:5218/getReport",
            //"http://localhost:5228/",
        };

int numberOfRequests = args.Length > 0 ? int.Parse(args[0]) : 10000;
int degreeOfParallelism = args.Length > 1 ? int.Parse(args[1]) : 100;

Console.WriteLine($"DDoS simulation has been started. Number of Request/Degree Of Parallel...({numberOfRequests}/{degreeOfParallelism})");

foreach (var url in urls)
{
    Console.WriteLine($"Starting DDoS simulation on {url}");
    await StartDdosAttack(url, numberOfRequests, degreeOfParallelism);
    Console.WriteLine($"Simulation completed for {url}");
}

Console.WriteLine("All simulations completed...");

async Task StartDdosAttack(string url, int numberOfRequests, int degreeOfParallelism)
{
    Task[] tasks = new Task[degreeOfParallelism];

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
                    Console.WriteLine($"Request {reqId}: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request {reqId} failed: {ex.Message}");
                }
            }
        });
    }

    await Task.WhenAll(tasks);
}