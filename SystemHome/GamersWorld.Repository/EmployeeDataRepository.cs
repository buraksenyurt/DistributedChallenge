using Dapper;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Domain.Entity;
using Microsoft.Extensions.Logging;
using Npgsql;
using SecretsAgent;

namespace GamersWorld.Repository;

public class EmployeeDataRepository(ISecretStoreService secretStoreService, ILogger<ReportDataRepository> logger)
    : IEmployeeDataRepository
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<ReportDataRepository> _logger = logger;
    const string createEmployee = @"
                INSERT INTO employee (full_name, email, title, registration_id, password_hash) 
                VALUES (@FullName, @Email, @Title, @RegistrationId, @PasswordHash)
                RETURNING employee_id";
    const string selectEmployeeByRegistrationId = @"
                SELECT employee_id,full_name, email, title, registration_id, password_hash
                FROM employee
                WHERE registration_id = @RegistrationId";

    private async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connStr = await _secretStoreService.GetSecretAsync("GamersWorldDbConnStr");
        var dbConnection = new NpgsqlConnection(connStr);
        await dbConnection.OpenAsync();

        return dbConnection;
    }

    public async Task<int> Register(Employee employee)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var insertedId = await dbConnection.ExecuteScalarAsync<int>(createEmployee, new
        {
            employee.Fullname,
            employee.Title,
            employee.Email,
            employee.RegistrationId,
            employee.PasswordHash
        });
        if (insertedId == 1)
        {
            _logger.LogInformation("New employee created with {InsertedId}", insertedId);
        }
        else
        {
            _logger.LogError("Problem on new employee creation");
        }

        return insertedId;
    }

    public async Task<Employee> Get(string registrationId)
    {
        await using var dbConnection = await GetOpenConnectionAsync();
        var queryResult = await dbConnection.QueryAsync(selectEmployeeByRegistrationId, new { RegistrationId = registrationId });

        return queryResult.Select(r => new Employee
        {
            EmployeeId = r.employee_id,
            Fullname = r.full_name,
            Title = r.title,
            Email = r.email,
            RegistrationId = r.registration_id,
            PasswordHash = r.password_hash
        }).First();
    }
}