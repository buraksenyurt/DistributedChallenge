using GamersWorld.Business.Contracts;

namespace GamersWorld.Business.Concretes;

public class FileSaver
    : IDocumentSaver
{
    public async Task<int> SaveTo(string sourceName, byte[] data)
    {
        //QUESTION : Diyelimki dosyanın yazıldığı disk dolu veya arızalandı. Sistem nasıl tepki vermeli?

        await File.WriteAllBytesAsync(
                    Path.Combine(Environment.CurrentDirectory, $"{sourceName}.csv")
                    , data);
        return data.Length;
    }
}