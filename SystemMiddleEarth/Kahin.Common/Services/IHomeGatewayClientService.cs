using Kahin.Common.Requests;

namespace Kahin.Common.Services;

public interface IHomeGatewayClientService
{
    Task<string> Post(string url, ReportStatusRequest request);
}