namespace VGStandard.Application.Common.Web.Cache;

public class NoCacheService : ICacheService
{
    public bool TryGetValue<T>(string key, out T value)
    {
        value = default!;
        return false;
    }

    public void Set(string key, object? data, int? cacheTime)
    {
    }

    public void Remove(string key)
    {
    }

    public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fetchData, int? cacheTime = null)
    {
        return fetchData();
    }

    public T GetOrSet<T>(string key, Func<T> fetchData, int? cacheTime = null)
    {
        return fetchData();
    }
}

