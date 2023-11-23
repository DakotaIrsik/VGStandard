using VaultSharp;


namespace VGStandard.Common.Web.Services
{
    public interface IVaultService
    {
        Task<Dictionary<string, string>> ReadSecretAsync(string secretPath, int? version = null, string mountPoint = "secret");
    }

    public class VaultService : IVaultService
    {
        private readonly IVaultClient _vaultClient;

        public VaultService(IVaultClient vaultClient)
        {
            _vaultClient = vaultClient;
        }

        public async Task<Dictionary<string, string>> ReadSecretAsync(string secretPath, int? version = null, string mountPoint = "secret")
        {
            var secrets =  await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath, version, mountPoint);
            return secrets.Data.Data.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? string.Empty);
        }
    }
}
