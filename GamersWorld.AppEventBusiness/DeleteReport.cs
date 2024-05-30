using GamersWorld.AppEvents;
using GamersWorld.Common.Messages.Responses;
using GamersWorld.SDK;
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
        _logger.LogWarning("{} is deleting from system",appEvent.CreatedReportId);

        // Dokümanı Local Storage'dan silme operasyonunu gerçekleştir
        throw new NotImplementedException();
    }
}
