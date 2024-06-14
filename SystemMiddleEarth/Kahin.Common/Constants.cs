namespace Kahin.Common.Constants;

public static class SecretName
{
    public const string HomeGatewayApiAddress = "HomeGatewayApiAddress";
    public const string RedisConnectionString = "RedisConnectionString";
    public const string EvalServiceApiAddress = "EvalServiceApiAddress";
}

public static class EncodingContent
{
    public const string Json = "application/json";
}

public static class Names
{
    public const string EvalApi = "EvalApi";
    public const string SourceDomain = "KahinDomain";
    public const string EventStream = "reportStream";
    public const string EventStreamField = "events";
}

public static class TimeCop
{
    public const int SleepDuration = 10_000;
    public const byte WaitFactor = 23;
    public const byte SixtyMinutes = 60;
    public const short OneMilisecond = 1000;
}