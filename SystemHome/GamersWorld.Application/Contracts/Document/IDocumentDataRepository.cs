using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Requests;

namespace GamersWorld.Application.Contracts.Document;

public interface IDocumentDataRepository
{
    Task<int> InsertDocumentAsync(DocumentSaveRequest documentSaveRequest);
    Task<Domain.Data.Document> ReadDocumentAsync(GenericDocumentRequest documentReadRequest);
    Task<int> GetDocumentLength(GenericDocumentRequest documentReadRequest);
    Task<IEnumerable<Domain.Data.Document>> GetAllDocumentsAsync();
    Task<IEnumerable<Domain.Data.Document>> GetAllDocumentsByEmployeeAsync(GenericDocumentRequest documentReadRequest);
    Task<DocumentContent> ReadDocumentContentByIdAsync(GenericDocumentRequest documentReadRequest);
    Task<int> DeleteDocumentByIdAsync(GenericDocumentRequest documentReadRequest);
    Task<IEnumerable<string>> GetExpiredDocumentsAsync();
    Task<int> MarkDocumentToArchiveAsync(GenericDocumentRequest documentReadRequest);
}
