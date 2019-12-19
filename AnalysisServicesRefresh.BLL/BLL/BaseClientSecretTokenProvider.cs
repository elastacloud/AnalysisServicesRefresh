using AnalysisServicesRefresh.BLL.Helpers;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public abstract class BaseClientSecretTokenProvider : ITokenProvider
    {
        protected readonly ModelConfiguration Model;

        protected BaseClientSecretTokenProvider(ModelConfiguration model)
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
            var clientSecret = Model.Authentication.KeyVaultAuthentication;
            var secretNames = new List<string> { ClientIdName, ClientSecretName };

            var secrets = await AuthenticationHelpers.GetSecretsFromKeyVault(
                keyVaultBaseUri, clientId, clientSecret, secretNames, cancellationToken
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
    }
}