using GamersWorld.AppEvents;
using GamersWorld.SDK;
using GamersWorld.SDK.Messages;
using Microsoft.Extensions.Logging;

namespace GamersWorld.AppEventBusiness;

public class DeleteReport
    : IEventDriver<ReportProcessCompletedEvent>
{
    private readonly ILogger<DeleteReport> _logger;
    public DeleteReport(ILogger<DeleteReport> logger)
    {
        _logger = logger;
    }
    public async Task<BusinessResponse> Execute(ReportProcessCompletedEvent appEvent)
    {
        //TODO@buraksenyurt Must implement Report Process Completed actions
        _logger.LogWarning($"{appEvent.CreatedReportId} is deleting from system");

        // Dokümanı Local Storage'dan silme operasyonunu gerçekleştir
        throw new NotImplementedException();
    }
}
