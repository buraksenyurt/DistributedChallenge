using Dapper;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Data;
using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Requests;
using Microsoft.Extensions.Logging;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class DocumentDataRepository(ISecretStoreService secretStoreService, ILogger<DocumentDataRepository> logger)
    : IDocumentDataRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<DocumentDataRepository> _logger = logger;

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
                INSERT INTO Documents (TraceId, ReportTitle, EmployeeId, DocumentId, Content, InsertTime, ExpireTime)
                VALUES (@TraceId, @ReportTitle, @EmployeeId, @DocumentId, @Content, @InsertTime, @ExpireTime)
                RETURNING Id";

        await using var dbConnection = await GetOpenConnectionAsync();
        var insertedId = await dbConnection.ExecuteScalarAsync<int>(sql, new
        {
            documentSaveRequest.TraceId,
            documentSaveRequest.ReportTitle,
            documentSaveRequest.EmployeeId,
            documentSaveRequest.DocumentId,
            documentSaveRequest.Content,
            documentSaveRequest.InsertTime,
            documentSaveRequest.ExpireTime
        });

        return insertedId;
    }

    public async Task<Document> ReadDocumentAsync(GenericDocumentRequest documentReadRequest)
    {
        const string sql = @"
                SELECT Id, TraceId, ReportTitle, EmployeeId, DocumentId, Content, InsertTime, ExpireTime
                FROM Documents
                WHERE DocumentId = @DocumentId AND Archived = False";

        await using var dbConnection = await GetOpenConnectionAsync();
        var reportDocument = await dbConnection.QueryFirstOrDefaultAsync<Document>(sql, new { documentReadRequest.DocumentId });
        if (reportDocument == null)
        {
            _logger.LogInformation("There is no content for {DocumentId}", documentReadRequest.DocumentId);
            return new Document
            {
                DocumentId = documentReadRequest.DocumentId
            };
        }
        return reportDocument;
    }

    public async Task<DocumentContent> ReadDocumentContentByIdAsync(GenericDocumentRequest documentReadRequest)
    {
        const string sql = @"
                SELECT Content
                FROM Documents
                WHERE DocumentId = @DocumentId AND Archived = False";

        await using var dbConnection = await GetOpenConnectionAsync();
        var content = await dbConnection.QueryFirstOrDefaultAsync<byte[]>(sql, new { documentReadRequest.DocumentId });

        if (content == null)
        {
            _logger.LogInformation("There is no content for {DocumentId}", documentReadRequest.DocumentId);
            return new DocumentContent
            {
                Base64Content = string.Empty,
                ContentSize = 0
            };
        }
        return new DocumentContent
        {
            Base64Content = Convert.ToBase64String(content),
            ContentSize = content.Length
        };
    }

    public async Task<int> GetDocumentLength(GenericDocumentRequest documentReadRequest)
    {
        const string sql = @"
                SELECT LENGTH(Content) AS ContentLength
                FROM Documents
                WHERE DocumentId = @DocumentId AND Archived = False";

        await using var dbConnection = await GetOpenConnectionAsync();
        var length = await dbConnection.QueryFirstOrDefaultAsync<int>(sql, new { documentReadRequest.DocumentId });

        return length;
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
    {
        const string sql = @"
                SELECT Id, TraceId, ReportTitle, EmployeeId, DocumentId, Content, InsertTime, ExpireTime
                FROM Documents
                ORDER BY InsertTime AND Archived = False";

        await using var dbConnection = await GetOpenConnectionAsync();
        var documents = await dbConnection.QueryAsync<Document>(sql);

        return documents;
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsByEmployeeAsync(GenericDocumentRequest documentReadRequest)
    {
        const string sql = @"
                SELECT Id, TraceId, ReportTitle, EmployeeId, DocumentId, Content, InsertTime, ExpireTime
                FROM Documents
                WHERE EmployeeId = @EmployeeId AND Archived = False
                ORDER BY InsertTime";

        await using var dbConnection = await GetOpenConnectionAsync();
        var documents = await dbConnection.QueryAsync<Document>(sql, new { documentReadRequest.EmployeeId });

        return documents;
    }

    public async Task<int> DeleteDocumentByIdAsync(GenericDocumentRequest documentReadRequest)
    {
        const string sql = @"
                DELETE
                FROM Documents
                WHERE DocumentId = @DocumentId";

        await using var dbConnection = await GetOpenConnectionAsync();
        var affectedRowCount = await dbConnection.ExecuteAsync(sql, new { documentReadRequest.DocumentId });
        return affectedRowCount;
    }

    public async Task<int> MarkDocumentToArchiveAsync(GenericDocumentRequest documentReadRequest)
    {
        const string sql = @"
                UPDATE
                Documents
                SET Archived = true
                WHERE DocumentId = @DocumentId";

        await using var dbConnection = await GetOpenConnectionAsync();
        var affectedRowCount = await dbConnection.ExecuteAsync(sql, new { documentReadRequest.DocumentId });
        return affectedRowCount;
    }

    public async Task<IEnumerable<string>> GetExpiredDocumentsAsync()
    {
        const string sql = @"
                SELECT DocumentId
                FROM Documents
                WHERE ExpireTime >= NOW() AND Archived = True";
        await using var dbConnection = await GetOpenConnectionAsync();
        var documentIdList = await dbConnection.QueryAsync<string>(sql);
        return documentIdList;
    }
}