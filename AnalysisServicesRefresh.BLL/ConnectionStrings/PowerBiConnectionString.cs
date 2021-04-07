using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.ConnectionStrings
{
    public class PowerBiConnectionString : IConnectionString
    {
        private readonly ICredentialFactory _credentialFactory;

        public PowerBiConnectionString() : this(new CredentialFactory())
        {
        }

        public PowerBiConnectionString(ICredentialFactory credentialFactory)
        {
            _credentialFactory = credentialFactory;
        }

        public async Task<string> GetAsync(ModelConfiguration model, CancellationToken cancellationToken = default)
        {
            var clientId = model.Authentication.AnalysisServicesClientId;
            var clientSecret = model.Authentication.AnalysisServicesClientSecret;

            var credential = await _credentialFactory.Create(model.Authentication)
                .GetAsync(clientId, clientSecret, cancellationToken);

            return
                $"Data Source={model.ServerName};User ID=app:{credential.Username}@{model.Authentication.DirectoryId};Password={credential.Password};";
        }
    }
}