using GamersWorld.Common.Enums;

namespace GamersWorld.Common.Responses;

public class BusinessResponse
{
    public StatusCode StatusCode { get; set; }
    public string Message { get; set; } = "Business Response Message";
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
    public object? Data { get; set; } // Burada Object türü kullanmak iyi bir yaklaşım değil. Generic tercih edilbilir mi?
}