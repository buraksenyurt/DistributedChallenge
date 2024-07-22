using Dapper;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Domain.Data;
using GamersWorld.Domain.Requests;
using Microsoft.Extensions.Logging;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class ReportDataRepository(ISecretStoreService secretStoreService, ILogger<ReportDataRepository> logger)
    : IReportDataRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<ReportDataRepository> _logger = logger;
    const string insertReport= @"
                INSERT INTO report (trace_id, title, employee_id, document_id, insert_time, expire_time)
                VALUES (@TraceId, @Title, @EmployeeId, @DocumentId, @InsertTime, @ExpireTime)
                RETURNING report_id";
    const string selectReport= @"
                SELECT report_id, trace_id, title, employee_id, document_id, insert_time, expire_time
                FROM report
                WHERE document_id = @DocumentId";
    const string selectAllReport = @"
                SELECT report_id, trace_id, title, employee_id, document_id, insert_time, expire_time
                FROM report
                ORDER BY insert_time AND archived = False";
    const string selectReportByEmployee = @"
                SELECT report_id, trace_id, title, employee_id, document_id, insert_time, expire_time
                FROM report
                WHERE employee_id = @EmployeeId AND archived = False
                ORDER BY insert_time";
    const string updateReport = @"
                UPDATE
                report
                SET archived = true
                WHERE document_id = @DocumentId";
    const string softDeleteReport = @"
                UPDATE
                report
                SET deleted = true
                WHERE document_id = @DocumentId";
    const string selectExpiredReport = @"
                SELECT document_id
                FROM report
                WHERE expire_time <=  @AdjustedTime AND archived = False"; // Expired but not marked as archived
    const string selectReportOnRemove = @"
            SELECT document_id
            FROM report
            WHERE expire_time <= @AdjustedTime AND archived = True AND deleted = False"; // Marked as archived and the expiretime ended with a delayed interval


    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("GamersWorldDbConnStr");
        var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();
        return dbConnection;
    }

    public async Task<int> InsertReportAsync(ReportSaveRequest requestData)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var insertedId = await dbConnection.ExecuteScalarAsync<int>(insertReport, new
        {
            requestData.EmployeeId,
            requestData.Title,
            requestData.TraceId,
            requestData.DocumentId,
            requestData.InsertTime,
            requestData.ExpireTime
        });

        return insertedId;
    }

    public async Task<Report> ReadReportAsync(GenericDocumentRequest requestData)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var queryResult = await dbConnection.QueryAsync(selectReport, new { requestData.DocumentId });

        if (queryResult == null)
        {
            _logger.LogInformation("There is no content for {DocumentId}", requestData.DocumentId);
            return new Report
            {
                DocumentId = requestData.DocumentId
            };
        }

        return queryResult.Select(r => new Report
        {
            DocumentId = r.report_id,
            Title = r.title,
            TraceId = r.trace_id,
            EmployeeId = r.employee_id,
            ExpireTime = r.expire_time,
            InsertTime = r.insert_time,
            ReportId = r.report_id
        }).First();
    }

    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var queryResult = await dbConnection.QueryAsync(selectAllReport);

        return queryResult.Select(r => new Report
        {
            EmployeeId = r.employee_id,
            DocumentId = r.document_id,
            ExpireTime = r.expire_time,
            InsertTime = r.insert_time,
            ReportId = r.report_id,
            Title = r.title,
            TraceId = r.trace_id
        });
    }

    public async Task<IEnumerable<Report>> GetAllReportsByEmployeeAsync(GenericDocumentRequest requestData)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var queryResult = await dbConnection.QueryAsync(selectReportByEmployee, new { requestData.EmployeeId });

        return queryResult.Select(r => new Report
        {
            EmployeeId = r.employee_id,
            DocumentId = r.document_id,
            ExpireTime = r.expire_time,
            InsertTime = r.insert_time,
            ReportId = r.report_id,
            Title = r.title,
            TraceId = r.trace_id
        });
    }

    public async Task<int> MarkReportToArchiveAsync(GenericDocumentRequest requestData)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var affectedRowCount = await dbConnection.ExecuteAsync(updateReport, new { requestData.DocumentId });

        return affectedRowCount;
    }

    public async Task<int> MarkReportAsDeletedAsync(GenericDocumentRequest requestData)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var affectedRowCount = await dbConnection.ExecuteAsync(softDeleteReport, new { requestData.DocumentId });

        return affectedRowCount;
    }

    public async Task<IEnumerable<string>> GetExpiredReportsAsync()
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var documentIdList = await dbConnection.QueryAsync<string>(selectExpiredReport, new { AdjustedTime = DateTime.Now });

        return documentIdList;
    }

    public async Task<IEnumerable<string>> GetReportsOnRemoveAsync(TimeSpan interval)
    {
        var adjustedTime = DateTime.Now - interval;

        await using var dbConnection = await GetOpenConnectionAsync();
        var documentIdList = await dbConnection.QueryAsync<string>(selectReportOnRemove, new { AdjustedTime = adjustedTime });

        return documentIdList;
    }
}