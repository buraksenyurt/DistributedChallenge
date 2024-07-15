using Microsoft.Extensions.Logging;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Notification;
using System.Text.Json;
using GamersWorld.Domain.Dtos;

namespace GamersWorld.EventBusiness;

public class InvalidExpression(ILogger<InvalidExpression> logger, INotificationService notificationService) : IEventDriver<InvalidExpressionEvent>
{
    private readonly ILogger<InvalidExpression> _logger = logger;
    private readonly INotificationService _notificationService = notificationService;

    public async Task Execute(InvalidExpressionEvent appEvent)
    {
        var notificationData = new ReportNotification
        {
            DocumentId = "Audit Validation Error !",
            Content = appEvent.Title,
            IsSuccess = false,
            Topic = Domain.Enums.NotificationTopic.Report.ToString(),
        };
        await _notificationService.PushToUserAsync(appEvent.EmployeeId, JsonSerializer.Serialize(notificationData));

        _logger.LogWarning("{ReportTitle}, Reason: {Reason}", appEvent.Title, appEvent.Reason);
    }
}
