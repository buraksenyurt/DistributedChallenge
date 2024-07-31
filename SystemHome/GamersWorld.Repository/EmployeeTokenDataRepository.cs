using Dapper;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Domain.Entity;
using Microsoft.Extensions.Logging;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class EmployeeTokenDataRepository(ISecretStoreService secretStoreService, ILogger<EmployeeTokenDataRepository> logger)
    : IEmployeeTokenDataRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<EmployeeTokenDataRepository> _logger = logger;

    const string insertOrUpdate = @"
        INSERT INTO employee_tokens (registration_id, token, insert_time, expire_time)
        VALUES (@RegistrationId, @Token, @InsertTime, @ExpireTime)
        ON CONFLICT (registration_id)
        DO UPDATE SET
            token = EXCLUDED.token,
            insert_time = EXCLUDED.insert_time,
            expire_time = EXCLUDED.expire_time;
    ";

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("GamersWorldDbConnStr");
        var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();

        return dbConnection;
    }
    public async Task<int> UpsertAsync(EmployeeToken employeeToken)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var affected = await dbConnection.ExecuteAsync(insertOrUpdate, new
        {
            employeeToken.RegistrationId,
            employeeToken.Token,
            employeeToken.InsertTime,
            employeeToken.ExpireTime
        });
        if (affected > 0)
        {
            _logger.LogInformation("{Affected} rows affected.", affected);
        }
        else
        {
            _logger.LogError("Problem on sql upsert operation");
        }

        return affected;
    }
}