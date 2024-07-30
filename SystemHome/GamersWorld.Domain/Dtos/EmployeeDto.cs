namespace GamersWorld.Domain.Dtos;

public record EmployeeDto
{
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public string RegistrationId { get; set; }
    public string Password { get; set; }
}