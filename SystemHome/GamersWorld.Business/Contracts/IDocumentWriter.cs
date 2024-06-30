using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;

namespace GamersWorld.Business.Contracts;

public interface IDocumentWriter
{
    Task<BusinessResponse> SaveAsync(DocumentSaveRequest payload);
}