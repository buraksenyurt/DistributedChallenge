using Microsoft.Extensions.Logging;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Notification;
using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Enums;
using System.Text.Json;
using GamersWorld.Application.Contracts.Data;

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
        var requestData = new Domain.Requests.GenericDocumentRequest
        {
            DocumentId = appEvent.DocumentId,
        };
        var deleteResult = await _reportDocumentDataRepository.DeleteDocumentByIdAsync(requestData);
        if (deleteResult == 1)
        {
            var archived = await _reportDataRepository.MarkReportToArchiveAsync(requestData);
            if (archived == 1)
            {
                _logger.LogInformation("{DocumentId} content has been deleted and main report archived.", appEvent.DocumentId);
                var notificationData = new ReportNotification
                {
                    DocumentId = appEvent.DocumentId,
                    Content = appEvent.Title,
                    Topic = NotificationTopic.Deleted.ToString(),
                    IsSuccess = true,
                };

                await Task.Delay(2000);

                await _notificationService.PushToUserAsync(appEvent.ClientId, JsonSerializer.Serialize(notificationData));
            }
        }
    }
}
