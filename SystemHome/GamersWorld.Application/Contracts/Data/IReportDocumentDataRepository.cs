using GamersWorld.Domain.Data;
using GamersWorld.Domain.Requests;

namespace GamersWorld.Application.Contracts.Data;

public interface IReportDocumentDataRepository
{
    Task<int> InsertReportDocumentAsync(ReportDocumentSaveRequest requestData);
    Task<int> GetDocumentLength(GenericDocumentRequest requestData);
    Task<ReportDocument?> ReadDocumentByIdAsync(GenericDocumentRequest requestData);
    Task<int> DeleteDocumentByIdAsync(GenericDocumentRequest requestData);
}
