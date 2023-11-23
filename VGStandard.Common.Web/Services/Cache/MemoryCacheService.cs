using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VGStandard.Core.Settings;

namespace VGStandard.Application.Common.Web.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly AppSettings _settings;
    private readonly ILogger _logger;

    public MemoryCacheService(IMemoryCache cache, IOptions<AppSettings> settings, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _settings = settings.Value;
        _logger = logger;
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Set(string key, object data, int? cacheTime = null)
    {
        _cache.Set(key, data, new TimeSpan(0, 0, cacheTime ?? _settings.CacheSettings.CacheDurationInSeconds));
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fetchData, int? cacheTime = null)
    {
        _logger.LogInformation($"{this.GetType().Name.Replace("CacheService", "")} Cache lookup for Key: {key}");
        if (TryGetValue(key, out T value))
        {
            _logger.LogInformation($"{this.GetType().Name.Replace("CacheService", "")} Cache lookup SUCCESS: {key}");
            return value;
        }
        _logger.LogInformation($"{this.GetType().Name.Replace("CacheService", "")} Cache lookup FAILURE: {key}");
        value = await fetchData();
        Set(key, value, cacheTime ?? _settings.CacheSettings.CacheDurationInSeconds);
        return value;
    }
    public T GetOrSet<T>(string key, Func<T> fetchData, int? cacheTime = null)
    {
        _logger.LogInformation($"{this.GetType().Name.Replace("CacheService", "")} Cache lookup for Key: {key}");
        if (TryGetValue(key, out T value))
        {
            _logger.LogInformation($"{this.GetType().Name.Replace("CacheService", "")} Cache lookup SUCCESS: {key}");
            return value;
        }
        _logger.LogInformation($"{this.GetType().Name.Replace("CacheService", "")} Cache lookup FAILURE: {key}");
        value = fetchData();
        Set(key, value, cacheTime ?? _settings.CacheSettings.CacheDurationInSeconds);
        return value;
    }
}

