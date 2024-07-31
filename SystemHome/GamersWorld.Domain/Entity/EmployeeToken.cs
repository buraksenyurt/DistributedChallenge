namespace GamersWorld.Domain.Entity;

public class EmployeeToken
{
    public string RegistrationId { get; set; }
    public string Token { get; set; }
    public DateTime InsertTime { get; set; }
    public DateTime ExpireTime { get; set; }
}