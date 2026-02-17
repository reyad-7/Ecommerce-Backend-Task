using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Interfaces.ICacheService;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.RedisCacheService
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly int _defaultTTLMinutes;

        public RedisCacheService(IDistributedCache cache, int defaultTTLMinutes = 30)
        {
            _cache = cache;
            _defaultTTLMinutes = defaultTTLMinutes;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(key);

                if (string.IsNullOrEmpty(cachedData))
                    return null;

                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(value);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(_defaultTTLMinutes)
                };

                await _cache.SetStringAsync(key, serializedData, options);
            }
            catch (Exception)
            {
                // Log error in production
                // Don't throw - cache failures shouldn't break the app
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception)
            {
                // Log error in production
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
