using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;

namespace GamersWorld.Application.Contracts.Document;

public interface IDocumentWriter
{
    Task<BusinessResponse> SaveAsync(DocumentSaveRequest payload);
}