using Newtonsoft.Json;

namespace VGStandard.Core.Settings;

public class AppSettings
{

    public bool RecreatePostgresTables { get; set; }
    public string AppHostname { get; set; }
    public int? MaxConcurrentUpgradedConnections { get; set; } = null;
    public int? MaxConcurrentConnections { get; set; } = null;

    public string Environment { get; set; }
    public string Application { get; set; }
    //TODO JWT Auth
    public string AuthenticationStrategy { get; set; } = "Cookie";

    public string PoorManClientCredential { get; set; }
    public string WithoutBboxPath { get; set; }
    public string ElasticSearchApiKey { get; set; }

    public bool BulkPostgres { get; set; }
    public bool BulkElasticSearch { get; set; }

    public string CertPass => @"vRrVg72gaYNdGF9rgQSJ";

    [Obsolete]
    public Dictionary<string, Dictionary<string, object>> ConnectedUsers = new Dictionary<string, Dictionary<string, object>>();
    public CacheSetting CacheSettings { get; set; } = new();
    public ConnectionString ConnectionStrings { get; set; } = new();
    public Token Tokens { get; set; } = new();

    public UserToken UserTokens { get; set; } = new();
    public List<Integration> Integrations { get; set; } = new();
    public string LogLevel { get; set; } = "Error";
    public class CacheSetting
    {
        public string CacheType { get; set; }
        public int CacheDurationInSeconds { get; set; }
        public string ConnectionString { get; set; }
    }

    public class Token
    {
        public string Vault { get; set; }
    }

    public class ConnectionString
    {
        public string Postgres { get; set; }
        public string Redis { get; set; }
        public string ElasticSearch { get; set; }
        public string SignalRHub { get; set; }
        public string Vault { get; set; }
    }

    public class Integration
    {
        public string EndPoint { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string AppId { get; set; }
    }

    public class UserToken
    {
        public string Key { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInDays { get; set; }
    }

    public class ConnectionPooling
    {
        public bool Enabled { get; set; } = false;
        public int? MaxConnectionPoolSize { get; set; } = null;
        public int? ConnectionIdleLifeTime { get; set; } = null;
        public int? ConnectionPruningInterval { get; set; } = null;

    }

    public JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
    };

    public bool SkipPostgres { get; set; }
    public bool SkipElasticsearch { get; set; }
}