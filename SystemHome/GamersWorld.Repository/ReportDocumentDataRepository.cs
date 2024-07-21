using Dapper;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Domain.Data;
using GamersWorld.Domain.Requests;
using Microsoft.Extensions.Logging;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class ReportDocumentDataRepository(ISecretStoreService secretStoreService, ILogger<ReportDataRepository> logger)
    : IReportDocumentDataRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<ReportDataRepository> _logger = logger;

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("GamersWorldDbConnStr");
        var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();
        return dbConnection;
    }

    public async Task<int> InsertReportDocumentAsync(ReportDocumentSaveRequest requestData)
    {
        const string sql = @"
                INSERT INTO report_document (fk_report_id, content)
                VALUES (@ReportId, @Content)
                RETURNING report_document_id";

        await using var dbConnection = await GetOpenConnectionAsync();
        var insertedId = await dbConnection.ExecuteScalarAsync<int>(sql, new
        {
            requestData.ReportId,
            requestData.Content
        });

        return insertedId;
    }

    public async Task<ReportDocument?> ReadDocumentByIdAsync(GenericDocumentRequest requestData)
    {
        const string sql = @"
                SELECT content 
                FROM report_document rd JOIN report r 
                ON rd.fk_report_id = r.report_id
                WHERE r.document_id = @DocumentId";

        await using var dbConnection = await GetOpenConnectionAsync();
        var reportDocument = await dbConnection.QueryFirstOrDefaultAsync<ReportDocument>(sql, new { requestData.DocumentId });
        return reportDocument;
    }

    public async Task<int> GetDocumentLength(GenericDocumentRequest requestData)
    {
        const string sql = @"
                SELECT LENGTH(content) AS ContentLength 
                FROM report_document rd JOIN report r 
                ON rd.fk_report_id = r.report_id
                WHERE r.document_id = @DocumentId";

        await using var dbConnection = await GetOpenConnectionAsync();
        var length = await dbConnection.QueryFirstOrDefaultAsync<int>(sql, new { requestData.DocumentId });

        return length;
    }

    public async Task<int> DeleteDocumentByIdAsync(GenericDocumentRequest requestData)
    {
        const string sql = @"
                DELETE
                FROM report_document
                WHERE fk_report_id = (SELECT report_id FROM report WHERE document_id = @DocumentId)";

        await using var dbConnection = await GetOpenConnectionAsync();
        var affectedRowCount = await dbConnection.ExecuteAsync(sql, new { requestData.DocumentId });
        return affectedRowCount;
    }
}