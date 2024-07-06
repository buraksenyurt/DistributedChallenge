using GamersWorld.Domain.Data;
using GamersWorld.Domain.Requests;

namespace GamersWorld.Application.Contracts.Document;

public interface IDocumentRepository
{
    Task<int> InsertDocumentAsync(DocumentSaveRequest documentSaveRequest);
    Task<Domain.Data.Document> ReadDocumentAsync(DocumentReadRequest documentReadRequest);
    Task<int> GetDocumentLength(DocumentReadRequest documentReadRequest);
    Task<IEnumerable<Domain.Data.Document>> GetAllDocumentsAsync();
    Task<IEnumerable<Domain.Data.Document>> GetAllDocumentsByEmployeeAsync(DocumentReadRequest documentReadRequest);
    Task<DocumentContent> ReadDocumentContentByIdAsync(DocumentReadRequest documentReadRequest);
}
