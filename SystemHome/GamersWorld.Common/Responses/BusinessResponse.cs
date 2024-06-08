using GamersWorld.Common.Enums;

namespace GamersWorld.Common.Messages.Responses;

public class BusinessResponse
{
    public StatusCode StatusCode { get; set; }
    public string Message { get; set; } = "Business Response Message";
}