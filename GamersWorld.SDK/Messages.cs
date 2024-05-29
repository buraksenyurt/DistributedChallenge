using GamersWorld.SDK.Enums;

namespace GamersWorld.SDK.Messages;

public class BusinessResponse
{
    public StatusCode StatusCode { get; set; }
    public string Message { get; set; } = "Business Response Message";
}