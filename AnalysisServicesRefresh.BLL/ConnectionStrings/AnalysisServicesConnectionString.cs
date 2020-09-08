using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Tokens;

namespace AnalysisServicesRefresh.BLL.ConnectionStrings
{
    public class AnalysisServicesConnectionString : IConnectionString
    {
        private readonly ICredentialFactory _credentialFactory;
        private readonly ITokenFactory _tokenFactory;

        public AnalysisServicesConnectionString() : this(new CredentialFactory(), new TokenFactory())
        {
        }

        public AnalysisServicesConnectionString(ICredentialFactory credentialFactory,
            ITokenFactory tokenFactory)
        {
            _credentialFactory = credentialFactory;
            _tokenFactory = tokenFactory;
        }

        public async Task<string> GetAsync(ModelConfiguration model, CancellationToken cancellationToken = default)
        {
            var clientIdName = model.Authentication.AnalysisServicesClientIdName;
            var clientSecretName = model.Authentication.AnalysisServicesClientSecretName;

            var credential = await _credentialFactory.Create(model.Authentication)
                .GetAsync(clientIdName, clientSecretName, cancellationToken);

            var authority = $"https://login.microsoftonline.com/{model.Authentication.DirectoryId}";
            const string resource = "https://*.asazure.windows.net/";

            var token = await _tokenFactory.Create(credential, authority, resource)
                .GetAsync(cancellationToken);

            return $"Data Source={model.ServerName};Password={token.AccessToken};Provider=MSOLAP;";
        }
    }
}