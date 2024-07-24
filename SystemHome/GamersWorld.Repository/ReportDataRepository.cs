using Dapper;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Domain.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class ReportDataRepository(ISecretStoreService secretStoreService, ILogger<ReportDataRepository> logger)
    : IReportDataRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<ReportDataRepository> _logger = logger;
    const string createReport = @"
                INSERT INTO report (trace_id, title, expression, employee_id, document_id, insert_time, expire_time, archived, deleted)
                VALUES (@TraceId, @Title, @Expression, @EmployeeId, @DocumentId, @InsertTime, @ExpireTime, @Archived, @Deleted)
                RETURNING report_id";
    const string selectReportByDocumentId = @"
                SELECT report_id, trace_id, title, expression, employee_id, document_id, insert_time, expire_time, archived, deleted
                FROM report
                WHERE document_id = @DocumentId";
    const string selectAllReport = @"
                SELECT report_id, trace_id, title, expression, employee_id, document_id, insert_time, expire_time, archived, deleted
                FROM report
                ORDER BY insert_time AND archived = False";
    const string selectReportByEmployeeId = @"
                SELECT report_id, trace_id, title, expression, employee_id, document_id, insert_time, expire_time, archived, deleted
                FROM report
                WHERE employee_id = @EmployeeId AND archived = False
                ORDER BY insert_time";
    const string selectReport = @"
                SELECT document_id
                FROM report
                WHERE expire_time <=  @AdjustedTime AND archived = False"; // Expired but not marked as archived
    const string selectExpiredReport = @"
                SELECT document_id
                FROM report
                WHERE expire_time <= @AdjustedTime AND archived = True AND deleted = False"; // Marked as archived and the expiretime ended with a delayed interval
    const string updateReport = @"
                UPDATE
                report
                SET      
                    trace_id = @TraceId,
                    employee_id = @EmployeeId,
                    title = @Title,
                    expression = @Expression,
                    insert_time = @InsertTime,
                    expire_time = @ExpireTime,
                    deleted = @Deleted,
                    archived = @Archived
                WHERE report_id = @ReportId";

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("GamersWorldDbConnStr");
        var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();

        return dbConnection;
    }

    public async Task<int> CreateReportAsync(Report report)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var insertedId = await dbConnection.ExecuteScalarAsync<int>(createReport, new
        {
            report.EmployeeId,
            report.Title,
            report.Expression,
            report.TraceId,
            report.DocumentId,
            report.InsertTime,
            report.ExpireTime,
            report.Archived,
            report.Deleted
        });

        return insertedId;
    }

    public async Task<Report> ReadReportAsync(string documentId)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var queryResult = await dbConnection.QueryAsync(selectReportByDocumentId, new { DocumentId = documentId });

        if (queryResult == null)
        {
            _logger.LogInformation("There is no content for {DocumentId}", documentId);
            return new Report
            {
                DocumentId = documentId
            };
        }

        return queryResult.Select(r => new Report
        {
            DocumentId = r.document_id,
            Title = r.title,
            Expression = r.expression,
            TraceId = r.trace_id,
            EmployeeId = r.employee_id,
            ExpireTime = r.expire_time,
            InsertTime = r.insert_time,
            ReportId = r.report_id,
            Archived = r.archived,
            Deleted = r.deleted
        }).First();
    }

    public async Task<IEnumerable<Report>> ReadAllReportsAsync()
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var queryResult = await dbConnection.QueryAsync(selectAllReport);

        return queryResult.Select(r => new Report
        {
            DocumentId = r.document_id,
            Title = r.title,
            Expression = r.expression,
            TraceId = r.trace_id,
            EmployeeId = r.employee_id,
            ExpireTime = r.expire_time,
            InsertTime = r.insert_time,
            ReportId = r.report_id,
            Archived = r.archived,
            Deleted = r.deleted
        });
    }

    public async Task<IEnumerable<Report>> ReadAllReportsAsync(string employeeId)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var queryResult = await dbConnection.QueryAsync(selectReportByEmployeeId, new { EmployeeId = employeeId });

        return queryResult.Select(r => new Report
        {
            DocumentId = r.document_id,
            Title = r.title,
            Expression = r.expression,
            TraceId = r.trace_id,
            EmployeeId = r.employee_id,
            ExpireTime = r.expire_time,
            InsertTime = r.insert_time,
            ReportId = r.report_id,
            Archived = r.archived,
            Deleted = r.deleted
        });
    }

    public async Task<IEnumerable<string>> GetExpiredReportsAsync()
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var documentIdList = await dbConnection.QueryAsync<string>(selectReport, new { AdjustedTime = DateTime.Now });

        return documentIdList;
    }

    public async Task<IEnumerable<string>> GetExpiredReportsAsync(TimeSpan interval)
    {
        var adjustedTime = DateTime.Now - interval;

        await using var dbConnection = await GetOpenConnectionAsync();
        var documentIdList = await dbConnection.QueryAsync<string>(selectExpiredReport, new { AdjustedTime = adjustedTime });

        return documentIdList;
    }

    public async Task<int> UpdateReportAsync(Report report)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var affectedRowCount = await dbConnection.ExecuteAsync(updateReport, new
        {
            report.TraceId,
            report.EmployeeId,
            report.Title,
            report.Expression,
            report.InsertTime,
            report.ExpireTime,
            report.Deleted,
            report.Archived,
            report.ReportId
        });

        _logger.LogInformation("Updated row count is {AffectedRowCount}", affectedRowCount);

        return affectedRowCount;
    }
}