using GamersWorld.Domain.Data;

namespace GamersWorld.Application.Contracts.Data;

public interface IReportDocumentDataRepository
{
    Task<int> CreateReportDocumentAsync(ReportDocument reportDocument);
    Task<int> GetDocumentLength(string documentId);
    Task<ReportDocument?> ReadDocumentAsync(string documentId);
    Task<int> DeleteDocumentAsync(string documentId);
}
