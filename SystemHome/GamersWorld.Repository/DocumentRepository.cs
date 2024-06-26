using Dapper;
using GamersWorld.Common.Requests;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class DocumentRepository(ISecretStoreService secretStoreService)
    : IDocumentRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    //QUESTION int döndürmek yerine daha anlamlı bir veri yapısı dönülemez mi?
    public async Task<int> InsertDocumentAsync(DocumentSaveRequest documentSaveRequest)
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");

        using var dbConnection = new NpgsqlConnection(connStr);
        dbConnection.Open();

        var sql = @"
                INSERT INTO Documents (TraceId, EmployeeId, DocumentId, Content)
                VALUES (@TraceId, @EmployeeId, @DocumentId, @Content)
                RETURNING Id";

        var insertedId = await dbConnection.ExecuteScalarAsync<int>(sql, new
        {
            documentSaveRequest.TraceId,
            documentSaveRequest.EmployeeId,
            documentSaveRequest.DocumentId,
            documentSaveRequest.Content
        });

        return insertedId;
    }

    //QUESTION byte[] array döndürmek yerine daha anlamlı bir veri yapısı dönülemez mi?

    public async Task<byte[]> ReadDocumentAsync(DocumentReadRequest documentReadRequest)
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");

        using var dbConnection = new NpgsqlConnection(connStr);
        dbConnection.Open();

        var sql = @"
                SELECT Content FROM Documents
                WHERE DocumentId = @DocumentId";

        var content = await dbConnection.QueryFirstOrDefaultAsync<byte[]>(sql, new { documentReadRequest.DocumentId });

        return content;
    }
}