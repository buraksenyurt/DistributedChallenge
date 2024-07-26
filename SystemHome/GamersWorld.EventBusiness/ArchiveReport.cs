using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Notification;
using GamersWorld.Domain.Constants;
using GamersWorld.Domain.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GamersWorld.EventBusiness;

public class ArchiveReport(
    ILogger<ArchiveReport> logger
    , IReportDocumentDataRepository reportDocumentDataRepository
    , IReportDataRepository reportDataRepository
    , IServiceProvider serviceProvider
    , INotificationService notificationService)
    : IEventDriver<ArchiveReportRequestEvent>
{
    private readonly ILogger<ArchiveReport> _logger = logger;
    private readonly IReportDocumentDataRepository _reportDocumentDataRepository = reportDocumentDataRepository;
    private readonly IReportDataRepository _reportDataRepository = reportDataRepository;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly INotificationService _notificationService = notificationService;

    public async Task Execute(ArchiveReportRequestEvent appEvent)
    {
        var doc = await _reportDocumentDataRepository.ReadDocumentAsync(appEvent.DocumentId);
        if (doc == null)
        {
            _logger.LogWarning("{DocumentId} content not found", appEvent.DocumentId);
            return;
        }
        var writeOperator = _serviceProvider.GetRequiredKeyedService<IDocumentWriter>(Names.FtpWriteService);

        var writeResponse = await writeOperator.SaveAsync(new Domain.Requests.ReportSaveRequest
        {
            DocumentId = appEvent.DocumentId,
            Content = doc.Content,
        });
        if (writeResponse.Status != Domain.Enums.Status.DocumentUploaded)
        {
            _logger.LogWarning("{DocumentId} save operation failed", appEvent.DocumentId);
            return;
        }

        var report = await _reportDataRepository.ReadReportAsync(appEvent.DocumentId);
        report.Archived = true;
        var updateResult = await _reportDataRepository.UpdateReportAsync(report);
        if (updateResult == 1)
        {
            var notificationData = new ReportNotificationDto
            {
                DocumentId = appEvent.DocumentId,
                Content = appEvent.Title,
                IsSuccess = true,
                Topic = Domain.Enums.NotificationTopic.Archived.ToString()
            };

            await _notificationService.PushToUserAsync(appEvent.ClientId, JsonSerializer.Serialize(notificationData));
            _logger.LogWarning("{Document} is archiving to target", appEvent.DocumentId);
        }
        else
        {
            _logger.LogWarning("{DocumentId} marked as archive from db operation failed", appEvent.DocumentId);
        }
    }
}
