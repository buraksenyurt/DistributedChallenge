using GamersWorld.Domain.Entity;

namespace GamersWorld.Application.Contracts.Data;

public interface IEmployeeTokenDataRepository
{
    Task<int> UpsertAsync(EmployeeToken employeeToken);
}