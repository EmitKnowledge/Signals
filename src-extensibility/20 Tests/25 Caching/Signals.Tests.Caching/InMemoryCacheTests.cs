using Signals.Aspects.Caching.Enums;
using Signals.Aspects.Caching.InMemory;
using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.Caching.InMemory.Entries;
using Signals.Tests.Caching.Models;
using System;
using System.Threading;
using Xunit;

namespace Signals.Tests.Caching
{
    public class InMemoryCacheTests
    {
        private InMemoryCacheConfiguration InitConfig()
        {
            var config = new InMemoryCacheConfiguration();
            config.ExpirationTime = TimeSpan.FromMinutes(5);
            config.ExpirationPolicy = CacheExpirationPolicy.Sliding;

            return config;
        }

        [Fact]
        public void CachingEntry_Cached_Exists()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value_1";
            var value = new CachedModel();
            _cache.Set(key, value);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Equal(value.Value, cachedValue.Value);
        }

        [Fact]
        public void CachingEntry_Recached_Exists()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value_2";

            var value = new CachedModel { Value = 1 };
            _cache.Set(key, value, TimeSpan.FromHours(1));
            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Equal(value.Value, cachedValue.Value);

            var value2 = "value2";
            _cache.Set(key, value2, TimeSpan.FromHours(1));
            var cachedValue2 = _cache.Get<string>(key);
            Assert.Equal(value2, cachedValue2);
        }

        [Fact]
        public void CachingEntry_CachedAndExpired_DoesntExist()
        {
            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(0);

            var _cache = new InMemoryCache(config);

            var key = "test_value_3";
            var value = new CachedModel();
            _cache.Set(key, value);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Null(cachedValue);
        }

        [Fact]
        public void CachingEntry_CachedAndRenewed_Exist()
        {
            var expirationTime = 300;
            var timeout = 200;

            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(expirationTime);

            var _cache = new InMemoryCache(config);

            var key = "test_value_4";
            var value = new CachedModel();
            _cache.Set(key, value);

            Thread.Sleep(timeout);

            // renew sliding expiration by _cache.Get
            var cachedValue = _cache.Get<CachedModel>(key);

            Thread.Sleep(timeout);

            cachedValue = _cache.Get<CachedModel>(key);
            Assert.NotNull(cachedValue);
            Assert.Equal(value.Value, cachedValue.Value);
        }

        [Fact]
        public void CachingEntry_Invalidated_DoesntExist()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value_5";
            var value = new CachedModel();
            _cache.Set(key, value);

            _cache.Invalidate(key);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Null(cachedValue);
        }

        [Fact]
        public void CachingEntry_UnexistantInvalidated_Exists()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value_6";
            var unexistantKey = "test_value_7";
            var value = new CachedModel();
            _cache.Set(key, value);

            _cache.Invalidate(unexistantKey);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.NotNull(cachedValue);
        }


        [Fact]
        public void CachingReloadableEntry_Cached_Exists()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value_8";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () => { value.Value++; return value; });
            _cache.Set(entry);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Equal(value.Value, cachedValue.Value);
        }

        [Fact]
        public void CachingReloadableEntry_CachedAndExpired_ExistsByReload()
        {
            var expirationTime = 300;
            var timeout = 400;

            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(expirationTime);

            var _cache = new InMemoryCache(config);

            var key = "test_value_9";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () =>
            {
                value.Value++;
                return value;
            });
            _cache.Set(entry);

            Thread.Sleep(timeout);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.NotNull(cachedValue);
            Assert.Equal(value.Value, cachedValue.Value);
            Assert.Equal(2, value.Value);
        }

        [Fact]
        public void CachingReloadableEntry_CachedAndRenewed_Exist()
        {
            var expirationTime = 300;
            var timeout = 200;

            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(expirationTime);

            var _cache = new InMemoryCache(config);

            var key = "test_value_10";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () => { value.Value++; return value; });
            _cache.Set(entry);

            Thread.Sleep(timeout);

            var cachedValue = _cache.Get<CachedModel>(key);

            Thread.Sleep(timeout);

            cachedValue = _cache.Get<CachedModel>(key);
            Assert.NotNull(cachedValue);
        }

        [Fact]
        public void CachingReloadableEntry_Invalidated_DoesntExist()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value_11";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () => { value.Value++; return value; });
            _cache.Set(entry);

            _cache.Invalidate(key);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Null(cachedValue);
        }

        [Fact]
        public void CachingReloadableEntry_UnexistantInvalidated_Exists()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value_12";
            var unexistantKey = "test_value_13";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () => { value.Value++; return value; });
            _cache.Set(entry);

            _cache.Invalidate(unexistantKey);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.NotNull(cachedValue);
        }
    }
}
