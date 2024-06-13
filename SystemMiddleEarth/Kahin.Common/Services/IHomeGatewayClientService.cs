using Kahin.Common.Requests;

namespace Kahin.Common.Services;

public interface IHomeGatewayClientService
{
    Task<string> Post(ReportStatusRequest request);
}