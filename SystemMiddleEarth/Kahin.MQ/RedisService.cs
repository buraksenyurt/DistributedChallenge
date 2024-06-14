using System.Text.Json;
using Kahin.Common.Constants;
using Kahin.Common.Requests;
using Kahin.Common.Services;
using StackExchange.Redis;

namespace Kahin.MQ;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ISecretStoreService _secretStoreService;

    public RedisService(ISecretStoreService secretStoreService)
    {
        _secretStoreService = secretStoreService;
        var connectionString = _secretStoreService.GetSecretAsync(SecretName.RedisConnectionString).GetAwaiter().GetResult();
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public async Task AddReportPayloadAsync(string streamName, RedisPayload payload, TimeSpan? lifetime = null)
    {
        var jsonPayload = JsonSerializer.Serialize(payload);
        await _db.StreamAddAsync(streamName, Names.EventStreamField, jsonPayload);
        if (lifetime.HasValue)
        {
            await _db.KeyExpireAsync(streamName, lifetime);
        }
    }

    public async Task<RedisPayload> Peek(string streamName)
    {
        var entries = await _db.StreamReadAsync(streamName, "0-0", 1);
        if (entries.Length > 0)
        {
            var jsonPayload = entries[0].Values[0].Value;
            var payload = JsonSerializer.Deserialize<RedisPayload>(jsonPayload);
            return payload ?? RedisPayload.Default();
        }

        return RedisPayload.Default();
    }

    public async Task<RedisPayload> Pop(string streamName)
    {
        var entries = await _db.StreamReadAsync(streamName, "0-0", 1);
        if (entries.Length > 0)
        {
            var jsonPayload = entries[0].Values[0].Value;
            await _db.StreamDeleteAsync(streamName, [entries[0].Id]);

            var payload = JsonSerializer.Deserialize<RedisPayload>(jsonPayload);
            return payload ?? RedisPayload.Default();
        }

        return RedisPayload.Default();
    }
}
