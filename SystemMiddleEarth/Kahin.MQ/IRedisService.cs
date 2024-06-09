using Kahin.Common.Requests;

namespace Kahin.MQ;

public interface IRedisService
{
    Task AddReportPayloadAsync(string streamName, RedisPayload redisPayload);
    Task<RedisPayload> ReadReportPayloadAsync(string streamName);
}
