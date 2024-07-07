using Dapper;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Data;
using GamersWorld.Domain.Requests;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class DocumentRepository(ISecretStoreService secretStoreService)
    : IDocumentRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("ReportDbConnStr");
        var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();
        return dbConnection;
    }

    public async Task<int> InsertDocumentAsync(DocumentSaveRequest documentSaveRequest)
    {
        const string sql = @"
                INSERT INTO Documents (TraceId, ReportTitle, EmployeeId, DocumentId, Content)
                VALUES (@TraceId, @ReportTitle, @EmployeeId, @DocumentId, @Content)
                RETURNING Id";

        await using var dbConnection = await GetOpenConnectionAsync();
        var insertedId = await dbConnection.ExecuteScalarAsync<int>(sql, new
        {
            documentSaveRequest.TraceId,
            documentSaveRequest.ReportTitle,
            documentSaveRequest.EmployeeId,
            documentSaveRequest.DocumentId,
            documentSaveRequest.Content
        });

        return insertedId;
    }

    public async Task<Document> ReadDocumentAsync(DocumentReadRequest documentReadRequest)
    {
        const string sql = @"
                SELECT Id, TraceId, ReportTitle, EmployeeId, DocumentId, Content, InsertTime
                FROM Documents
                WHERE DocumentId = @DocumentId";

        await using var dbConnection = await GetOpenConnectionAsync();
        var reportDocument = await dbConnection.QueryFirstOrDefaultAsync<Document>(sql, new { documentReadRequest.DocumentId });

        return reportDocument;
    }

    public async Task<DocumentContent> ReadDocumentContentByIdAsync(DocumentReadRequest documentReadRequest)
    {
        const string sql = @"
                SELECT Content
                FROM Documents
                WHERE DocumentId = @DocumentId";

        await using var dbConnection = await GetOpenConnectionAsync();
        var content = await dbConnection.QueryFirstOrDefaultAsync<byte[]>(sql, new { documentReadRequest.DocumentId });

        return new DocumentContent
        {
            Base64Content = Convert.ToBase64String(content),
            ContentSize = content.Length
        };
    }

    public async Task<int> GetDocumentLength(DocumentReadRequest documentReadRequest)
    {
        const string sql = @"
                SELECT LENGTH(Content) AS ContentLength
                FROM Documents
                WHERE DocumentId = @DocumentId";

        await using var dbConnection = await GetOpenConnectionAsync();
        var length = await dbConnection.QueryFirstOrDefaultAsync<int>(sql, new { documentReadRequest.DocumentId });

        return length;
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
    {
        const string sql = @"
                SELECT Id, TraceId, ReportTitle, EmployeeId, DocumentId, Content, InsertTime
                FROM Documents
                ORDER BY InsertTime";

        await using var dbConnection = await GetOpenConnectionAsync();
        var documents = await dbConnection.QueryAsync<Document>(sql);

        return documents;
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsByEmployeeAsync(DocumentReadRequest documentReadRequest)
    {
        const string sql = @"
                SELECT Id, TraceId, ReportTitle, EmployeeId, DocumentId, Content, InsertTime
                FROM Documents
                WHERE EmployeeId = @EmployeeId
                ORDER BY InsertTime";

        await using var dbConnection = await GetOpenConnectionAsync();
        var documents = await dbConnection.QueryAsync<Document>(sql, new { documentReadRequest.EmployeeId });

        return documents;
    }
}