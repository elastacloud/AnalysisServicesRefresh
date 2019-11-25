using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AnalysisServicesRefresh.BLL.Helpers
{
    internal static class AuthenticationHelpers
    {
        private static readonly HttpClient HttpClient = new HttpClient(new WinHttpHandler
        {
            DefaultProxyCredentials = CredentialCache.DefaultCredentials
        });

        public static async Task<Dictionary<string, string>> GetSecretsFromKeyVault(string keyVaultBaseUri,
            string clientId, string clientSecret, IEnumerable<string> secrets, CancellationToken cancellationToken)
        {
            var keyVaultClient = new KeyVaultClient(async (a, r, s) =>
                (await Authenticate(a, r, clientId, clientSecret)).AccessToken, HttpClient);

            return await GetCredentialsFromKeyVault(keyVaultClient, keyVaultBaseUri, secrets, cancellationToken);
        }

        public static async Task<Dictionary<string, string>> GetSecretsFromKeyVault(string keyVaultBaseUri,
            string clientId, X509Certificate2 certificate, IEnumerable<string> secrets,
            CancellationToken cancellationToken)
        {
            var keyVaultClient = new KeyVaultClient(async (a, r, s) =>
                (await Authenticate(a, r, clientId, certificate)).AccessToken, HttpClient);

            return await GetCredentialsFromKeyVault(keyVaultClient, keyVaultBaseUri, secrets, cancellationToken);
        }

        public static Task<AuthenticationResult> Authenticate(string authority, string resource, string clientId,
            string clientSecret)
        {
            var context = new AuthenticationContext(authority);
            return context.AcquireTokenAsync(resource, new ClientCredential(clientId, clientSecret));
        }

        private static Task<AuthenticationResult> Authenticate(string authority, string resource, string clientId,
            X509Certificate2 certificate)
        {
            var context = new AuthenticationContext(authority);
            return context.AcquireTokenAsync(resource, new ClientAssertionCertificate(clientId, certificate));
        }

        private static async Task<Dictionary<string, string>> GetCredentialsFromKeyVault(IKeyVaultClient client,
            string baseUri, IEnumerable<string> secrets, CancellationToken cancellationToken)
        {
            var tasks = secrets
                .Select(x => client.GetSecretAsync(baseUri, x, cancellationToken))
                .ToList();

            await Task.WhenAll(tasks);

            var dictionary = new Dictionary<string, string>();
            foreach (var task in tasks)
            {
                var result = await task;
                dictionary.Add(result.SecretIdentifier.Name, result.Value);
            }

            return dictionary;
        }
    }
}