using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Helpers;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public abstract class BaseCertificateTokenProvider : ITokenProvider
    {
        protected readonly ModelConfiguration Model;

        protected BaseCertificateTokenProvider(ModelConfiguration model)
        {
            Model = model;
        }

        protected abstract string Authority { get; }
        protected abstract string ClientIdName { get; }
        protected abstract string ClientSecretName { get; }
        protected abstract string Resource { get; }

        public async Task<Token> CreateAsync(CancellationToken cancellationToken = default)
        {
            var keyVaultBaseUri = Model.Authentication.KeyVaultBaseUri;
            var clientId = Model.Authentication.KeyVaultClientId;
            var thumbprint = Model.Authentication.KeyVaultAuthentication;
            var certificate = GetCertificate(thumbprint);
            var secretNames = new List<string> {ClientIdName, ClientSecretName};

            var secrets = await AuthenticationHelpers.GetSecretsFromKeyVault(
                keyVaultBaseUri, clientId, certificate, secretNames, cancellationToken
            );

            var token = await AuthenticationHelpers.Authenticate(
                Authority, Resource, secrets[ClientIdName], secrets[ClientSecretName]
            );

            return new Token
            {
                AccessToken = token.AccessToken,
                ExpiresOn = token.ExpiresOn
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

                return certificate;
            }
        }
    }
}