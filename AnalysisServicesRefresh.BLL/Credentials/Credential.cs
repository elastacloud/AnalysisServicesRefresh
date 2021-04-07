using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.KeyVaults;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Credentials
{
    public class Credential : ICredential
    {
        private readonly AuthenticationConfiguration _authentication;
        private readonly IKeyVaultFactory _keyVaultFactory;

        public Credential(AuthenticationConfiguration authentication) : this(new KeyVaultFactory(), authentication)
        {
        }

        public Credential(IKeyVaultFactory keyVaultFactory, AuthenticationConfiguration authentication)
        {
            _keyVaultFactory = keyVaultFactory;
            _authentication = authentication;
        }

        public async Task<ClientCredential> GetAsync(string username, string password,
            CancellationToken cancellationToken = default)
        {
            return _authentication.KeyVaultBaseUri == null
                ? new ClientCredential
                {
                    Username = username,
                    Password = password
                }
                : await _keyVaultFactory.Create(_authentication.KeyVaultAuthenticationType, _authentication.KeyVaultBaseUri,
                        _authentication.KeyVaultClientId, _authentication.KeyVaultAuthentication)
                    .GetAsync(username, password, cancellationToken);
        }
    }
}