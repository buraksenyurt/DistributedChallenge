namespace GamersWorld.Domain.Dtos;

public record LoginDto
{
    public string RegistrationId { get; set; }
    public string Password { get; set; }
}