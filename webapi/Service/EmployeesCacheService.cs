using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace webapi
{
    public class EmployeesCacheService : ICache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public EmployeesCacheService(IMemoryCache memoryCache, IOptions<CacheOptions> cacheOptions)
        {
            _memoryCache = memoryCache;
            _cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheOptions.Value.AbsoluteExpirationMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(cacheOptions.Value.SlidingExpirationMinutes)
            };
        }

        public void Set<T>(string key, T value)
        {
            _memoryCache.Set(key, value, _cacheEntryOptions);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }

}
