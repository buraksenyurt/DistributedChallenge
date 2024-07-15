using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;

namespace GamersWorld.Application.Contracts.Document;

public interface IDocumentReader
{
    // Task<byte[]> ReadAsync(DocumentReadRequest payload);
    Task<BusinessResponse> GetLength(GenericDocumentRequest payload);
}