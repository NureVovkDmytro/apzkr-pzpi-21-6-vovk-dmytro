using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Discerniy.Infrastructure.Extensions.DistributedCache
{
    public static class CacheExtensions
    {
        public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> createItem, TimeSpan expiration)
        {
            return await cache.GetOrCreateAsync(key, createItem, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }

        public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> createItem, DistributedCacheEntryOptions options)
        {
            var data = await cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(data))
            {
                data = JsonConvert.SerializeObject(await createItem());

                await cache.SetStringAsync(key, data, options);
            }

            return JsonConvert.DeserializeObject<T>(data) ?? throw new Exception($"Cache error on {nameof(GetOrCreateAsync)}");
        }
    }
}
