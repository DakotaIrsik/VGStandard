using Newtonsoft.Json;

namespace VGStandard.Core.Settings;

public class AppSettings
{
    public string DeepstreamLoginToken { get; set; } = string.Empty;
    public string AppHostname { get; set; }
    public string HubHostName { get; set; }

    public int? MaxConcurrentUpgradedConnections { get; set; } = null;
    public int? MaxConcurrentConnections { get; set; } = null;

    public string Environment { get; set; }
    public bool IsAirGapped { get; set; } = false;
    public string StartupStrategy { get; set; } = string.Empty;
    public string Application { get; set; }
    //TODO JWT Auth
    public string AuthenticationStrategy { get; set; } = "Cookie";

    public string PoorManClientCredential { get; set; }
    public string WithoutBboxPath { get; set; }
    public string ElasticSearchApiKey { get; set; }
    public string AlertsPath { get; set; }
    public string ThumbsPath { get; set; }
    public string StaticPath { get; set; }
    public string ZocDistributionList { get; set; }
    public int ThumbWidth { get; set; } = 160;
    public int ThumbHeight { get; set; } = 90;
    public bool SaveNonBBoxImageOnManager { get; set; } = true;
    public int CloudImageSizeReduction { get; set; } = 50;
    public string UserLockedOutAlertEmail { get; set; } = "dakota@zeroeyes.com";

    [Obsolete("These will be populated to env from kube")]
    public Dictionary<string, object> Secrets = new Dictionary<string, object>();

    public string CertPass => @"vRrVg72gaYNdGF9rgQSJ";

    [Obsolete]
    public Dictionary<string, Dictionary<string, object>> ConnectedUsers = new Dictionary<string, Dictionary<string, object>>();
    public CacheSetting CacheSettings { get; set; } = new();
    public ConnectionString ConnectionStrings { get; set; } = new();
    public ConnectionPooling ManualConnectionPoolingSettings { get; set; } = new();
    public Token Tokens { get; set; } = new();

    public MetadataSetting MetadataSettings { get; set; } = new();
    public UserToken UserTokens { get; set; } = new();
    public List<Integration> Integrations { get; set; } = new();
    public IdentityOption IdentityOptions { get; set; } = new();
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

    public class MetadataSetting
    {
        public string BoundingBoxColor { get; set; }
        public double BoundingBoxWidth { get; set; }
        public string RegionOfUninterestColor { get; set; }
        public double RegionOfUninterestWidth { get; set; }
    }

    public class IdentityOption
    {
        public bool LockoutOnFailure { get; set; }
        public int DefaultLockoutTimeSpanMin { get; set; } = 60;
        public int MaxFailedAccessAttempts { get; set; } = 5;
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
}