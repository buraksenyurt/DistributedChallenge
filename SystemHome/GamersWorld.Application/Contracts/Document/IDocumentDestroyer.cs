using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;

namespace GamersWorld.Application.Contracts.Document;

public interface IDocumentDestroyer
{
    Task<BusinessResponse> DeleteAsync(GenericDocumentRequest payload);
}