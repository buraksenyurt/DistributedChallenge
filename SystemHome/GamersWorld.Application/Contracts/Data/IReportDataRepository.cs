using GamersWorld.Domain.Entity;

namespace GamersWorld.Application.Contracts.Data;

public interface IReportDataRepository
{
    Task<int> CreateReportAsync(Report report);
    Task<Domain.Entity.Report> ReadReportAsync(string documentId);
    Task<IEnumerable<Domain.Entity.Report>> ReadAllReportsAsync();
    Task<IEnumerable<Domain.Entity.Report>> ReadAllReportsAsync(string employeeId);
    Task<int> UpdateReportAsync(Report report);
    //Task<int> MarkReportToArchiveAsync(GenericDocumentRequest requestData);
    //Task<int> MarkReportAsDeletedAsync(GenericDocumentRequest requestData);
    Task<IEnumerable<string>> GetExpiredReportsAsync(TimeSpan interval);
    Task<IEnumerable<string>> GetExpiredReportsAsync();
}
