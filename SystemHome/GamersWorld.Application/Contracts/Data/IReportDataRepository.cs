using GamersWorld.Domain.Entity;

namespace GamersWorld.Application.Contracts.Data;

public interface IReportDataRepository
{
    Task<int> CreateReportAsync(Report report);
    Task<Report> ReadReportAsync(string documentId);
    Task<IEnumerable<Report>> ReadAllReportsAsync();
    Task<IEnumerable<Report>> ReadAllReportsAsync(string employeeId);
    Task<int> UpdateReportAsync(Report report);
    Task<IEnumerable<string>> GetExpiredReportsAsync(TimeSpan interval);
    Task<IEnumerable<string>> GetExpiredReportsAsync();
}
