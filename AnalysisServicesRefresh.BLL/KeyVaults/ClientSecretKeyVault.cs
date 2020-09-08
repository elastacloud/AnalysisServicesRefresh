using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;
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

        public async Task<ActiveDirectoryClientCredential> GetAsync(string clientIdName, string clientSecretName,
            CancellationToken cancellationToken = default)
        {
            var keyVaultClient = new KeyVaultClient(async (authority, resource, s) =>
            {
                var context = new AuthenticationContext(authority);
                var token = await context.AcquireTokenAsync(resource,
                    new ClientCredential(_keyVaultClientId, _keyVaultClientSecret));
                return token.AccessToken;
            });

            var clientId = keyVaultClient.GetSecretAsync(_keyVaultBaseUri, clientIdName, cancellationToken);
            var clientSecret = keyVaultClient.GetSecretAsync(_keyVaultBaseUri, clientSecretName, cancellationToken);
            await Task.WhenAll(clientId, clientSecret);

            return new ActiveDirectoryClientCredential
            {
                ClientId = (await clientId).Value,
                ClientSecret = (await clientSecret).Value
            };
        }
    }
}