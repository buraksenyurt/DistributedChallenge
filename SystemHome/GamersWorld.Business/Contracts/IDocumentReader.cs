using GamersWorld.Common.Requests;
using GamersWorld.Common.Responses;

namespace GamersWorld.Business.Contracts;

public interface IDocumentReader
{
    // Task<byte[]> ReadAsync(DocumentReadRequest payload);
    Task<BusinessResponse> GetLength(DocumentReadRequest payload);
}