using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public class ClientSecretKeyVault : IKeyVault
    {
        private readonly string _keyVaultBaseUri;
        private readonly string _keyVaultClientId;
        private readonly string _keyVaultClientSecret;

        public ClientSecretKeyVault(string keyVaultBaseUri, string keyVaultClientId, string keyVaultClientSecret)
        {
            _keyVaultBaseUri = keyVaultBaseUri;
            _keyVaultClientId = keyVaultClientId;
            _keyVaultClientSecret = keyVaultClientSecret;
        }

        public async Task<Models.ClientCredential> GetAsync(string username, string password,
            CancellationToken cancellationToken = default)
        {
            var keyVaultClient = new KeyVaultClient(async (authority, resource, s) =>
            {
                var context = new AuthenticationContext(authority);
                var token = await context.AcquireTokenAsync(resource,
                    new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(_keyVaultClientId, _keyVaultClientSecret));
                return token.AccessToken;
            });

            var usernameTask = keyVaultClient.GetSecretAsync(_keyVaultBaseUri, username, cancellationToken);
            var passwordTask = keyVaultClient.GetSecretAsync(_keyVaultBaseUri, password, cancellationToken);
            await Task.WhenAll(usernameTask, passwordTask);

            return new Models.ClientCredential
            {
                Username = (await usernameTask).Value,
                Password = (await passwordTask).Value
            };
        }
    }
}