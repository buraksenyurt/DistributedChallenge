using GamersWorld.Business.Contracts;
using GamersWorld.Common.Requests;
using GamersWorld.Events;
using GamersWorld.MQ;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Business.Concretes;

public class FileSaver(ILogger<FileSaver> logger, IEventQueueService eventQueueService)
    : IDocumentSaver
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IEventQueueService _eventQueueService = eventQueueService;

    public async Task<int> SaveTo(DocumentSaveRequest payload)
    {
        if (payload == null || payload.Content == null)
        {
            return 0;
        }

        //QUESTION : Diyelimki dosyanın yazıldığı disk dolu veya arızalandı. Sistem nasıl tepki vermeli?
        try
        {
            await File.WriteAllBytesAsync(
                        Path.Combine(Environment.CurrentDirectory, $"{payload.DocumentId}.csv")
                        , payload.Content);


            var reportIsHereEvent = new ReportIsHereEvent
            {
                Time = DateTime.Now,
                TraceId = payload.TraceId,
                CreatedReportId = payload.DocumentId,
            };
            _eventQueueService.PublishEvent(reportIsHereEvent);

            return payload.Content.Length;
        }
        catch (Exception excp)
        {
            //QUESTION: Exception söz konusu ise, TraceId'ye sahip olaylar silsilesinin akibeti ne olacak?
            _logger.LogError(excp, "Error on document saving!");
            return 0;
        }
    }
}