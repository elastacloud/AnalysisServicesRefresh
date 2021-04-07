using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using NLog;
using Credential = Microsoft.AnalysisServices.Tabular.Credential;

namespace AnalysisServicesRefresh.BLL.DataSources
{
    public class UsernamePasswordDataSource : IDataSource
    {
        private readonly ICredentialFactory _credentialFactory;
        private readonly ILogger _logger;

        public UsernamePasswordDataSource() : this(new CredentialFactory(),
            LogManager.GetCurrentClassLogger())
        {
        }

        public UsernamePasswordDataSource(ICredentialFactory credentialFactory,
            ILogger logger)
        {
            _credentialFactory = credentialFactory;
            _logger = logger;
        }

        public async Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model,
            CancellationToken cancellationToken = default)
        {
            var dataSource = GetDataSource(database, model.DataSource.Name);
            dataSource.Credential = await GetCredential(model, cancellationToken);
        }

        private IStructuredDataSourceWrapper GetDataSource(IDatabaseWrapper database, string dataSourceName)
        {
            var dataSource = database.Model.DataSources.Find(dataSourceName);

            if (dataSource == null)
            {
                throw new ConnectionException($"Could not find data source {dataSourceName}.");
            }

            _logger.Info($"Fetched data source {dataSourceName}.");

            return dataSource;
        }

        private async Task<Credential> GetCredential(ModelConfiguration model,
            CancellationToken cancellationToken)
        {
            var username = model.DataSource.Username;
            var password = model.DataSource.Password;

            var credential = await _credentialFactory.Create(model.Authentication)
                .GetAsync(username, password, cancellationToken);

            return new Credential
            {
                AuthenticationKind = AuthenticationKind.UsernamePassword,
                Password = credential.Password,
                Username = credential.Username
            };
        }
    }
}