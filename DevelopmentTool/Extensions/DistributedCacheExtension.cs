using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevelopmentTool.Extensions;

public static class DistributedCacheExtension
{
    public static async Task<T?> GetValueAsync<T>(this IDistributedCache cache, string key) where T : class
    {
        var bytes = await cache.GetAsync(key);

        if (bytes is null)
        {
            return default;
        }

        var json = Encoding.Default.GetString(bytes);

        var result = JsonConvert.DeserializeObject<T>(json);

        return result;
    }
}