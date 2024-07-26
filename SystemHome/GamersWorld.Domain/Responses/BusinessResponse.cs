using GamersWorld.Domain.Enums;

namespace GamersWorld.Domain.Responses;

public class BusinessResponse
{
    public Status Status { get; set; }
    public string Message { get; set; } = "Business Response Message";
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}