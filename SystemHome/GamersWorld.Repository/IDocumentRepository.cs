using GamersWorld.Domain.Data;
using GamersWorld.Domain.Requests;

namespace GamersWorld.Repository;

public interface IDocumentRepository
{
    Task<int> InsertDocumentAsync(DocumentSaveRequest documentSaveRequest);
    Task<ReportDocument> ReadDocumentAsync(DocumentReadRequest documentReadRequest);
    Task<int> GetDocumentLength(DocumentReadRequest documentReadRequest);
    Task<IEnumerable<ReportDocument>> GetAllDocumentsAsync();
    Task<IEnumerable<ReportDocument>> GetAllDocumentsByEmployeeAsync(DocumentReadRequest documentReadRequest);
}
