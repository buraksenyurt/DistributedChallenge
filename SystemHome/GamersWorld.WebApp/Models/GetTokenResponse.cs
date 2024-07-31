namespace GamersWorld.WebApp.Models;

public record GetTokenResponse
{
    public string Token { get; set; }
    public string EmployeeFullname { get; set;}
    public string EmployeeTitle { get; set; }
}