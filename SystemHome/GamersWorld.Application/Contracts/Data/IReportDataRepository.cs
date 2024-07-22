using GamersWorld.Domain.Data;

namespace GamersWorld.Application.Contracts.Data;

public interface IReportDataRepository
{
    Task<int> CreateReportAsync(Report report);
    Task<Domain.Data.Report> ReadReportAsync(string documentId);
    Task<IEnumerable<Domain.Data.Report>> ReadAllReportsAsync();
    Task<IEnumerable<Domain.Data.Report>> ReadAllReportsAsync(string employeeId);
    Task<int> UpdateReportAsync(Report report);
    //Task<int> MarkReportToArchiveAsync(GenericDocumentRequest requestData);
    //Task<int> MarkReportAsDeletedAsync(GenericDocumentRequest requestData);
    Task<IEnumerable<string>> GetExpiredReportsAsync(TimeSpan interval);
    Task<IEnumerable<string>> GetExpiredReportsAsync();
}
