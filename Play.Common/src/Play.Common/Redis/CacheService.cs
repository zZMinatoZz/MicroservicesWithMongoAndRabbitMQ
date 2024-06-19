using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Play.Common.Interfaces;
using StackExchange.Redis;

namespace Play.Common.Redis
{

    public class CacheService : ICacheService
    {
        private readonly IDatabase cacheDb;
        public CacheService(IConnectionMultiplexer redis)
        {
            cacheDb = redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);
            return default;
        }

        public object RemoveData(string key)
        {
            var isExist = cacheDb.KeyExists(key);
            if (isExist) return cacheDb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }
    }
}