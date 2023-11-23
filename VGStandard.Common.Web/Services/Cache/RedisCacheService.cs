using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Security.Cryptography.X509Certificates;
using VGStandard.Common.Web.Services;
using VGStandard.Core.Settings;

namespace VGStandard.Application.Common.Web.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly int _defaultCacheDurationInSeconds;
    private readonly AppSettings _settings;
    private readonly ILogger _logger;
    private Dictionary<string, string>? redisSecrets;


    public ConfigurationOptions configuration => new ConfigurationOptions
    {
        EndPoints = { $"{_settings.ConnectionStrings.Redis}:6379"}, 
        ClientName = "VGStandard.Common.Web",
        Ssl = true,
        SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
        Password = "8fvxQsxTkYPJzPH",
        AbortOnConnectFail = false,
        User = "default",
        
    };

    public RedisCacheService(IOptions<AppSettings> options, IVaultService vaultService, ILogger<RedisCacheService> logger)
    {
        _settings = options.Value;
        _logger = logger;
        _defaultCacheDurationInSeconds = _settings.CacheSettings.CacheDurationInSeconds;

        //redisSecrets = vaultService.ReadSecretAsync("manager/template/settings/redis-cluster").Result;  //implement factory pattern
        configuration.CertificateSelection += (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) =>
        {
            return new X509Certificate2("ze.pfx", _settings.CertPass);
        };
        configuration.ConnectTimeout = 10000;
        var conn = ConnectionMultiplexer.Connect(configuration);
        LogConnectionStatus(conn);
        _database = conn.GetDatabase();
        _logger.LogInformation($"{_database.Ping()}");
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        string json = _database.StringGet(key);
        if (json == null)
        {
            value = default!;
            return false;
        }

        value = JsonConvert.DeserializeObject<T>(json);
        return true;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fetchData, int? cacheTime = null)
    {
        if (TryGetValue(key, out T value))
        {
            return value;
        }

        value = await fetchData();
        Set(key, value, cacheTime);
        return value;
    }

    public void Set(string key, object data, int? cacheTime = null)
    {
        string json = JsonConvert.SerializeObject(data);
        int duration = cacheTime ?? _defaultCacheDurationInSeconds;
        _database.StringSet(key, json, TimeSpan.FromSeconds(duration));
    }

    public void Remove(string key)
    {
        _database.KeyDelete(key);
    }

    public T GetOrSet<T>(string key, Func<T> fetchData, int? cacheTime = null)
    {
        if (TryGetValue(key, out T value))
        {
            return value;
        }

        value = fetchData();
        Set(key, value, cacheTime ?? _defaultCacheDurationInSeconds);
        return value;
    }

    private void LogConnectionStatus(ConnectionMultiplexer connection)
    {
        connection.ConnectionFailed += (sender, args) =>
        {
            _logger.LogError("Connection to Redis failed: {FailureType}, {Exception}", args.FailureType, args.Exception.Message);
        };

        connection.ConnectionRestored += (sender, args) =>
        {
            _logger.LogInformation("Connection to Redis restored: {FailureType}, {Exception}", args.FailureType, args.Exception.Message);
        };

        connection.ErrorMessage += (sender, args) =>
        {
            _logger.LogError("Received error message from Redis: {Message}", args.Message);
        };

        connection.InternalError += (sender, args) =>
        {
            _logger.LogError("Redis internal error: {Exception}", args.Exception.Message);
        };

        connection.ConfigurationChanged += (sender, args) =>
        {
            _logger.LogInformation("Redis configuration changed: {NewConfiguration}", args.EndPoint);
        };

        connection.ConfigurationChangedBroadcast += (sender, args) =>
        {
            _logger.LogInformation("Redis configuration changed broadcast: {NewConfiguration}", args.EndPoint);
        };
    }
}

