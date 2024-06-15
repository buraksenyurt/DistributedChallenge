using GamersWorld.Common.Requests;

namespace GamersWorld.Business.Contracts;

public interface IDocumentSaver
{
    Task<int> SaveTo(DocumentSaveRequest payload);
}