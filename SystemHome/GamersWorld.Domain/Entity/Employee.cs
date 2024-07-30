namespace GamersWorld.Domain.Entity;

public class Employee
{
    public int EmployeeId { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public string RegistrationId { get; set; }
    public string PasswordHash { get; set; }
}