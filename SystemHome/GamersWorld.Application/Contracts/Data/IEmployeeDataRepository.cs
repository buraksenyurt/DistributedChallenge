using GamersWorld.Domain.Entity;

namespace GamersWorld.Application.Contracts.Data;

public interface IEmployeeDataRepository
{
    Task<int> Register(Employee employee);
    Task<Employee> Get(string registrationId);
}