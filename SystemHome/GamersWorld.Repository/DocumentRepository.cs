using Dapper;
using GamersWorld.Common.Requests;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class DocumentRepository(ISecretStoreService secretStoreService)
    : IDocumentRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public async Task<int> InsertDocumentAsync(DocumentSaveRequest documentSaveRequest)
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");

        using var dbConnection = new NpgsqlConnection(connStr);
        dbConnection.Open();

        var sql = @"
                INSERT INTO Documents (TraceId, DocumentId, Content)
                VALUES (@TraceId, @DocumentId, @Content)
                RETURNING Id";

        var insertedId = await dbConnection.ExecuteScalarAsync<int>(sql, new
        {
            documentSaveRequest.TraceId,
            documentSaveRequest.DocumentId,
            documentSaveRequest.Content
        });

        return insertedId;
    }
}