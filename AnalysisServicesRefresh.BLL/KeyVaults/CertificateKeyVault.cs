using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public class CertificateKeyVault : IKeyVault
    {
        private readonly string _keyVaultBaseUri;
        private readonly string _keyVaultClientId;
        private readonly string _keyVaultThumbprint;

        public CertificateKeyVault(string keyVaultBaseUri, string keyVaultClientId, string keyVaultThumbprint)
        {
            _keyVaultBaseUri = keyVaultBaseUri;
            _keyVaultClientId = keyVaultClientId;
            _keyVaultThumbprint = keyVaultThumbprint;
        }

        public async Task<ActiveDirectoryClientCredential> GetAsync(string clientIdName, string clientSecretName,
            CancellationToken cancellationToken = default)
        {
            var certificate = GetCertificate(_keyVaultThumbprint);

            var keyVaultClient = new KeyVaultClient(async (authority, resource, s) =>
            {
                var context = new AuthenticationContext(authority);
                var token = await context.AcquireTokenAsync(resource,
                    new ClientAssertionCertificate(_keyVaultClientId, certificate));
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

        private static X509Certificate2 GetCertificate(string thumbprint)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                var certificate = store.Certificates.OfType<X509Certificate2>()
                    .FirstOrDefault(x => x.Thumbprint == thumbprint);

                store.Close();

                if (certificate == null)
                {
                    var certificates = store.Certificates.OfType<X509Certificate2>().Select(x => x.Thumbprint);

                    throw new InvalidOperationException(
                        $"Certificate with thumbprint {thumbprint} not found in My CurrentUser. Certificates found in My CurrentUser: {string.Join(", ", certificates)}");
                }

                return certificate;
            }
        }
    }
}