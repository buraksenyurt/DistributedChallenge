using Dapper;
using GamersWorld.Common.Data;
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

    public async Task<ReportDocument> ReadDocumentAsync(DocumentReadRequest documentReadRequest)
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");

        await using var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();

        var sql = @"
            SELECT Id, TraceId, EmployeeId, DocumentId, Content, InsertTime
            FROM Documents
            WHERE DocumentId = @DocumentId";

        var reportDocument = await dbConnection.QueryFirstOrDefaultAsync<ReportDocument>(sql, new { documentReadRequest.DocumentId });

        return reportDocument;
    }


    public async Task<int> GetDocumentLength(DocumentReadRequest documentReadRequest)
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");
        await using var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();
        var sql = @"
            SELECT LENGTH(Content) AS ContentLength
            FROM Documents
            WHERE DocumentId = @DocumentId";

        var length = await dbConnection.QueryFirstOrDefaultAsync<int>(sql, new { documentReadRequest.DocumentId });

        return length;
    }

    public async Task<IEnumerable<ReportDocument>> GetAllDocumentsAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");

        await using var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();

        var sql = @"
            SELECT Id, TraceId, EmployeeId, DocumentId, Content, InsertTime
            FROM Documents
            ORDER BY InsertTime";

        var documents = await dbConnection.QueryAsync<ReportDocument>(sql);

        return documents;
    }

    public async Task<IEnumerable<ReportDocument>> GetAllDocumentsByEmployeeAsync(DocumentReadRequest documentReadRequest)
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");

        await using var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();

        var sql = @"
            SELECT Id, TraceId, EmployeeId, DocumentId, Content, InsertTime
            FROM Documents
            WHERE EmployeeId = @EmployeeId
            ORDER BY InsertTime";

        var documents = await dbConnection.QueryAsync<ReportDocument>(sql, new { documentReadRequest.EmployeeId });

        return documents;
    }
}