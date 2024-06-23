using GamersWorld.Common.Requests;
using GamersWorld.Common.Responses;

namespace GamersWorld.Business.Contracts;

public interface IDocumentReader
{
    Task<BusinessResponse> ReadAsync(DocumentReadRequest payload);
}