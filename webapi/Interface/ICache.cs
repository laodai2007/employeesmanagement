using Microsoft.Extensions.Caching.Memory;

namespace webapi
{
    public interface ICache
    {
        void Set<T>(string key, T value);
        bool TryGetValue<T>(string key, out T value);
    }
}
