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

            var key = "test_value";
            var value = new CachedModel();
            _cache.Set(key, value);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Equal(value, cachedValue);
            Assert.Equal(value.Value, cachedValue.Value);
        }

        [Fact]
        public void CachingEntry_CachedAndExpired_DoesntExist()
        {
            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(0);

            var _cache = new InMemoryCache(config);

            var key = "test_value";
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

            var key = "test_value";
            var value = new CachedModel();
            _cache.Set(key, value);

            Thread.Sleep(timeout);

            // renew sliding expiration by _cache.Get
            var cachedValue = _cache.Get<CachedModel>(key);

            Thread.Sleep(timeout);

            cachedValue = _cache.Get<CachedModel>(key);
            Assert.NotNull(cachedValue);
            Assert.Equal(value, cachedValue);
        }

        [Fact]
        public void CachingEntry_Invalidated_DoesntExist()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value";
            var value = new CachedModel();
            _cache.Set(key, value);

            _cache.Invalidate(key);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Null(cachedValue);
        }

        [Fact]
        public void CachingEntry_InvalidatedAll_DoesntExist()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value";
            var value = new CachedModel();
            _cache.Set(key, value);

            _cache.Invalidate();

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Null(cachedValue);
        }


        [Fact]
        public void CachingReloadableEntry_Cached_Exists()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () => { value.Value++; return value; });
            _cache.Set(entry);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Equal(value, cachedValue);
        }

        [Fact]
        public void CachingReloadableEntry_CachedAndExpired_ExistsByReload()
        {
            var expirationTime = 300;
            var timeout = 400;

            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(expirationTime);

            var _cache = new InMemoryCache(config);

            var key = "test_value";
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
            Assert.Equal(value, cachedValue);
            Assert.Equal(2, value.Value);
        }

        [Fact]
        public void CachingReloadableEntry_CachedAndExpired_ExistsByReloadOnGet()
        {
            var expirationTime = 300;
            var timeout = 400;

            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(expirationTime);

            var _cache = new InMemoryCache(config);

            var key = "test_value";
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
            Assert.Equal(value, cachedValue);
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

            var key = "test_value";
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

            var key = "test_value";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () => { value.Value++; return value; });
            _cache.Set(entry);

            _cache.Invalidate(key);

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Null(cachedValue);
        }

        [Fact]
        public void CachingReloadableEntry_InvalidatedAll_DoesntExist()
        {
            var config = InitConfig();
            var _cache = new InMemoryCache(config);

            var key = "test_value";
            var value = new CachedModel();
            var entry = new ReloadableCacheEntry(key, () => { value.Value++; return value; });
            _cache.Set(entry);

            _cache.Invalidate();

            var cachedValue = _cache.Get<CachedModel>(key);
            Assert.Null(cachedValue);
        }


        [Fact]
        public void CachingEntry_Expiring_CallsCallbacks()
        {
            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(0);
            var _cache = new InMemoryCache(config);

            var key = "test_value";
            var key2 = "test_value2";
            var value = new CachedModel();
            _cache.Set(key, value);

            _cache.OnExpire(entry =>
            {
                Assert.NotNull(entry);
                Assert.Equal(value, entry.Value);
            });

            _cache.OnExpire(key, entry =>
            {
                Assert.NotNull(entry);
                Assert.Equal(value, entry.Value);
            });

            // this @key2 does not exist so the callback should never be fired
            // Assert.True(false) is force failing the test
            _cache.OnExpire(key2, entry =>
            {
                Assert.True(false);
            });
        }


        [Fact]
        public void CachingReloadableEntry_Expiring_CallsCallbacks()
        {
            var config = InitConfig();
            config.ExpirationTime = TimeSpan.FromMilliseconds(0);
            var _cache = new InMemoryCache(config);

            var key = "test_value";
            var key2 = "test_value2";
            var value = new CachedModel();
            var reloadableEntry = new ReloadableCacheEntry(key, () => value);
            _cache.Set(reloadableEntry);

            _cache.OnExpire(entry =>
            {
                Assert.NotNull(entry);
                Assert.Equal(value, entry.Value);
            });

            _cache.OnExpire(key, entry =>
            {
                Assert.NotNull(entry);
                Assert.Equal(value, entry.Value);
            });

            // this @key2 does not exist so the callback should never be fired
            // Assert.True(false) is force failing the test
            _cache.OnExpire(key2, entry =>
            {
                Assert.True(false);
            });
        }
    }
}
