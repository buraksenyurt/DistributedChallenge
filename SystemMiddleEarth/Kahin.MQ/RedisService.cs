using System.Text.Json;
using Kahin.Common.Requests;
using StackExchange.Redis;

namespace Kahin.MQ;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public async Task AddReportPayloadAsync(string streamName, RedisPayload payload)
    {
        var jsonPayload = JsonSerializer.Serialize(payload);
        await _db.StreamAddAsync(streamName, "events", jsonPayload);
    }

    public async Task<RedisPayload> ReadReportPayloadAsync(string streamName)
    {
        var entries = await _db.StreamReadAsync(streamName, "0-0", 1);
        if (entries.Length > 0)
        {
            var jsonPayload = entries[0].Values[0].Value;

            return JsonSerializer.Deserialize<RedisPayload>(jsonPayload);
        }

        return null;
    }
}
