using GamersWorld.Domain.Requests;

namespace GamersWorld.Application.Contracts.Data;

public interface IReportDataRepository
{
    Task<int> InsertReportAsync(ReportSaveRequest requestData);
    Task<Domain.Data.Report> ReadReportAsync(GenericDocumentRequest requestData);
    Task<IEnumerable<Domain.Data.Report>> GetAllReportsAsync();
    Task<IEnumerable<Domain.Data.Report>> GetAllReportsByEmployeeAsync(GenericDocumentRequest requestData);
    Task<int> MarkReportToArchiveAsync(GenericDocumentRequest requestData);
    Task<int> MarkReportAsDeletedAsync(GenericDocumentRequest requestData);
    Task<IEnumerable<string>> GetReportsOnRemoveAsync(TimeSpan interval);
    Task<IEnumerable<string>> GetExpiredReportsAsync();
}
