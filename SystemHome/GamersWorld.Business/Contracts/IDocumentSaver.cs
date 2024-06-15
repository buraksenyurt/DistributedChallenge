namespace GamersWorld.Business.Contracts;

public interface IDocumentSaver
{
    Task<int> SaveTo(string? sourceName, byte[] data);
}