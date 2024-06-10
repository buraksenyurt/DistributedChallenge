using Kahin.Common.Requests;

namespace Kahin.MQ;

public interface IRedisService
{
    Task AddReportPayloadAsync(string streamName, RedisPayload redisPayload, TimeSpan? lifetime = null);
    Task<RedisPayload> Peek(string streamName);
    Task<RedisPayload> Pop(string streamName);
}
