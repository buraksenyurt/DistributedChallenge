using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Notification;
using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GamersWorld.EventBusiness;

public class DeleteReport(
    ILogger<DeleteReport> logger
    , IReportDocumentDataRepository reportDocumentDataRepository
    , IReportDataRepository reportDataRepository
    , INotificationService notificationService
    ) : IEventDriver<DeleteReportRequestEvent>
{
    private readonly ILogger<DeleteReport> _logger = logger;
    private readonly IReportDocumentDataRepository _reportDocumentDataRepository = reportDocumentDataRepository;
    private readonly IReportDataRepository _reportDataRepository = reportDataRepository;
    private readonly INotificationService _notificationService = notificationService;

    public async Task Execute(DeleteReportRequestEvent appEvent)
    {
        _logger.LogInformation("{DocumentId} is deleting from system", appEvent.DocumentId);
        var deleteResult = await _reportDocumentDataRepository.DeleteDocumentAsync(appEvent.DocumentId);
        if (deleteResult == 1)
        {
            var report = await _reportDataRepository.ReadReportAsync(appEvent.DocumentId);
            report.Archived = true;
            report.Deleted = true;
            var updateResult = await _reportDataRepository.UpdateReportAsync(report);
            if (updateResult == 1)
            {
                _logger.LogInformation("{DocumentId} content has been deleted and main report marked as archived.", appEvent.DocumentId);
                var notificationData = new ReportNotificationDto
                {
                    DocumentId = appEvent.DocumentId,
                    Content = appEvent.Title,
                    Topic = NotificationTopic.Deleted.ToString(),
                    IsSuccess = true,
                };

                await Task.Delay(2000);

                await _notificationService.PushToUserAsync(appEvent.ClientId, JsonSerializer.Serialize(notificationData));
            }
            //TODO@buraksenyurt After fail we need publish a new event
        }
    }
}
