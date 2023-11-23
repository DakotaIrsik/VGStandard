namespace VGStandard.Application.Common.Web.Cache;

public interface ICacheService
{
    bool TryGetValue<T>(string key, out T value);
    void Set(string key, object? data, int? cacheTime = null);
    void Remove(string key);
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fetchData, int? cacheTime = null);
    T GetOrSet<T>(string key, Func<T> fetchData, int? cacheTime = null);
}
