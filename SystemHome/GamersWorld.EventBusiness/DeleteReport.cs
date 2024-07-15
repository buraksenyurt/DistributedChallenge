using Microsoft.Extensions.Logging;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Notification;
using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Enums;
using System.Text.Json;

namespace GamersWorld.EventBusiness;

public class DeleteReport(ILogger<DeleteReport> logger, IDocumentRepository documentRepository, INotificationService notificationService) : IEventDriver<DeleteReportRequestEvent>
{
    private readonly ILogger<DeleteReport> _logger = logger;
    private readonly IDocumentRepository _documentRepository = documentRepository;
    private readonly INotificationService _notificationService = notificationService;

    public async Task Execute(DeleteReportRequestEvent appEvent)
    {
        _logger.LogInformation("{DocumentId} is deleting from system", appEvent.DocumentId);
        var deleteResult = await _documentRepository.DeleteDocumentByIdAsync(new Domain.Requests.GenericDocumentRequest
        {
            DocumentId = appEvent.DocumentId,
        });
        if (deleteResult == 1)
        {
            _logger.LogInformation("{DocumentId} has been deleted from db", appEvent.DocumentId);

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
