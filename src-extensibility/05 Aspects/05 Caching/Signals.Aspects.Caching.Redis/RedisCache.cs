using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.Redis.Configurations;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace Signals.Aspects.Caching.Redis
{
    /// <summary>
    /// In memory cache implementation
    /// </summary>
    public class RedisCache : ICache
    {
        /// <summary>
        /// Cache configuration
        /// </summary>
        private readonly RedisCacheConfiguration Configuration;

        private readonly ConnectionMultiplexer Client;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public RedisCache(RedisCacheConfiguration configuration)
        {
            Configuration = configuration;
            if (Configuration.ConfigurationOptions != null)
            {
                Client = ConnectionMultiplexer.Connect(Configuration.ConfigurationOptions);
            }
            else
            {
                Client = ConnectionMultiplexer.Connect(Configuration.ConnectionEndpoint);
            }
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set<T>(string key, T value)
        {
            var entry = new CacheEntry(key, value);
            Set(entry);
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        public void Set<T>(string key, T value, TimeSpan expirationTime)
        {
            var entry = new CacheEntry(key, value);
            entry.ExpirationTime = expirationTime;
            Set(entry);
        }

        /// <summary>
        /// Cache value
        /// </summary>
        /// <param name="entry"></param>
        public void Set(CacheEntry entry)
        {
            entry.InvokeSet();

            entry.ExpirationTime = entry.ExpirationTime ?? Configuration.ExpirationTime;
            entry.ExpirationPolicy = entry.ExpirationPolicy ?? Configuration.ExpirationPolicy;

            var expirationTime = entry.ExpirationDate() - DateTime.UtcNow;

            if (expirationTime.TotalSeconds > 0)
            {
                ForDatabase(database =>
                {
                    database.StringSet(new RedisKey(entry.Key), new RedisValue(JsonConvert.SerializeObject(entry)), expirationTime, When.Always, CommandFlags.None);
                });
            }
        }

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            var entry = Get(key);
            if (entry == null) return null;

            if (entry.Value is T value)
            {
                return value;
            }
            else if (entry.Value is JObject jObject)
            {
                return jObject.ToObject<T>();
            }

            return null;
        }

        /// <summary>
        /// Get cached value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheEntry Get(string key)
        {
            var value = ForDatabase(database =>
            {
                return database.StringGet(key);
            });

            if (!value.HasValue) return null;

            var entry = JsonConvert.DeserializeObject<CacheEntry>(value);

            entry.InvokeGet();
            if (!entry.IsExpired())
            {
                Set(entry);
                return entry;
            }
            Invalidate(key);

            return null;
        }

        /// <summary>
        /// Invalidate cache entry
        /// </summary>
        /// <param name="key"></param>
        public void Invalidate(string key)
        {
            var entry = Get(key);
            if (entry == null) return;

            ForDatabase(database =>
            {
                database.KeyDelete(new RedisKey(key));
            });
        }

        private void ForDatabase(Action<IDatabase> action)
        {
            var database = Client.GetDatabase();
            action(database);
        }

        private T ForDatabase<T>(Func<IDatabase, T> action)
        {
            var database = Client.GetDatabase();
            return action(database);
        }
    }
}
