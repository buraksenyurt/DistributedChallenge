using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Application.Document;

public class FileSaver(ILogger<FileSaver> logger, IEventQueueService eventQueueService)
    : IDocumentWriter
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IEventQueueService _eventQueueService = eventQueueService;

    public async Task<BusinessResponse> SaveAsync(ReportSaveRequest payload)
    {
        if (payload == null || payload.Content == null)
        {
            _logger.LogError("Paylod or content is null");
            return new BusinessResponse
            {
                Status = Status.Fail,
                Message = "Payload or content is null"
            };
        }

        //QUESTION : Diyelimki dosyanın yazıldığı disk dolu veya arızalandı. Sistem nasıl tepki vermeli?
        try
        {
            await File.WriteAllBytesAsync(
                        Path.Combine(Environment.CurrentDirectory, $"{payload.DocumentId}.csv")
                        , payload.Content);


            var reportIsHereEvent = new ReportIsHereEvent
            {
                EventData = new BaseEventData
                {
                    TraceId = payload.TraceId,
                    Time = DateTime.Now
                },
                Title = payload.Title,
                Expression = payload.Expression,
                CreatedReportId = payload.DocumentId,
            };
            _eventQueueService.PublishEvent(reportIsHereEvent);

            return new BusinessResponse
            {
                Status = Status.DocumentSaved,
                Message = $"{payload.Content.Length} bytes saved."
            };
        }
        catch (Exception excp)
        {
            //QUESTION: Exception söz konusu ise, TraceId'ye sahip olaylar silsilesinin akibeti ne olacak?
            _logger.LogError(excp, "Error on document saving!");
            return new BusinessResponse
            {
                Status = Status.Fail,
                Message = $"Exception. {excp.Message}"
            };
        }
    }
}