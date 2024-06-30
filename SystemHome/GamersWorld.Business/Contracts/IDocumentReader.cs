using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;

namespace GamersWorld.Business.Contracts;

public interface IDocumentReader
{
    // Task<byte[]> ReadAsync(DocumentReadRequest payload);
    Task<BusinessResponse> GetLength(DocumentReadRequest payload);
}