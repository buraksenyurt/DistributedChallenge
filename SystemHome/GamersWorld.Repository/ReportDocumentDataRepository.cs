using Dapper;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Domain.Entity;
using Microsoft.Extensions.Logging;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class ReportDocumentDataRepository(ISecretStoreService secretStoreService, ILogger<ReportDataRepository> logger)
    : IReportDocumentDataRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<ReportDataRepository> _logger = logger;
    const string insertReportDocument = @"
                INSERT INTO report_document (fk_report_id, content)
                VALUES (@ReportId, @Content)
                RETURNING report_document_id";
    const string selectDocument = @"
                SELECT content 
                FROM report_document rd JOIN report r 
                ON rd.fk_report_id = r.report_id
                WHERE r.document_id = @DocumentId";
    const string selectDocumentLength = @"
                SELECT LENGTH(content) AS ContentLength 
                FROM report_document rd JOIN report r 
                ON rd.fk_report_id = r.report_id
                WHERE r.document_id = @DocumentId";
    const string deleteDocument = @"
                DELETE
                FROM report_document
                WHERE fk_report_id = (SELECT report_id FROM report WHERE document_id = @DocumentId)";

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("GamersWorldDbConnStr");
        var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();

        return dbConnection;
    }

    public async Task<int> CreateReportDocumentAsync(ReportDocument reportDocument)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var insertedId = await dbConnection.ExecuteScalarAsync<int>(insertReportDocument, new
        {
            reportDocument.ReportId,
            reportDocument.Content
        });

        _logger.LogInformation("New Report Document, {InsertedId} has been created", insertedId);

        return insertedId;
    }

    public async Task<ReportDocument?> ReadDocumentAsync(string documentId)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var reportDocument = await dbConnection.QueryFirstOrDefaultAsync<ReportDocument>(selectDocument, new { DocumentId = documentId });

        return reportDocument;
    }

    public async Task<int> GetDocumentLength(string documentId)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var length = await dbConnection.QueryFirstOrDefaultAsync<int>(selectDocumentLength, new { DocumentId = documentId });
        _logger.LogInformation("{DocumentId} length is {TotalBytes}", documentId, length);
        return length;
    }

    public async Task<int> DeleteDocumentAsync(string documentId)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var affectedRowCount = await dbConnection.ExecuteAsync(deleteDocument, new { DocumentId = documentId });

        _logger.LogInformation("{AffectedRowCount} Report Document has been deleted", affectedRowCount);

        return affectedRowCount;
    }
}