using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Kahin.Service.ReportingGateway;

public static class LoggingExtensions
{
    public static LoggerConfiguration UseElasticsearch(this LoggerConfiguration loggerConfiguration, string systemName, string environmentName)
    {
        return loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("System", systemName)
            .Enrich.WithProperty("Environment", environmentName)
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200")) //TODO@buraksenyurt Read via SecretService
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"kahin-reporting-service-logs-{environmentName.ToLower()}",
                TypeName = null,
                BatchAction = ElasticOpType.Create,
                ModifyConnectionSettings = x => x.ServerCertificateValidationCallback((sender, cert, chain, errors) => true)
            });
    }
}