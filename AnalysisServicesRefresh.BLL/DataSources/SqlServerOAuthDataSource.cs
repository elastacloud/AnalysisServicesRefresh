using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Tokens;
using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json;
using NLog;
using Credential = Microsoft.AnalysisServices.Tabular.Credential;
using Token = AnalysisServicesRefresh.BLL.Models.Token;

namespace AnalysisServicesRefresh.BLL.DataSources
{
    public class SqlServerOAuthDataSource : IDataSource
    {
        private readonly ICredentialFactory _credentialFactory;
        private readonly ILogger _logger;
        private readonly ITokenFactory _tokenFactory;

        public SqlServerOAuthDataSource() : this(new CredentialFactory(), new TokenFactory(),
            LogManager.GetCurrentClassLogger())
        {
        }

        public SqlServerOAuthDataSource(ICredentialFactory credentialFactory, ITokenFactory tokenFactory,
            ILogger logger)
        {
            _credentialFactory = credentialFactory;
            _tokenFactory = tokenFactory;
            _logger = logger;
        }

        public async Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model,
            CancellationToken cancellationToken = default)
        {
            var dataSource = GetDataSource(database, model.DataSource.Name);
            var sqlServerToken = await GetSqlServerToken(model, cancellationToken);
            dataSource.Credential = GetSqlServerCredential(sqlServerToken);
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

        private async Task<Token> GetSqlServerToken(ModelConfiguration model,
            CancellationToken cancellationToken)
        {
            var clientIdName = model.DataSource.ClientIdName;
            var clientSecretName = model.DataSource.ClientSecretName;

            var credential = await _credentialFactory.Create(model.Authentication)
                .GetAsync(clientIdName, clientSecretName, cancellationToken);

            var authority = $"https://login.windows.net/{model.Authentication.DirectoryId}";
            const string resource = "https://database.windows.net/";

            var token = await _tokenFactory.Create(credential, authority, resource)
                .GetAsync(cancellationToken);

            _logger.Info("Retrieved SQL Server authentication token.");

            return token;
        }

        private static Credential GetSqlServerCredential(Token sqlServerToken)
        {
            return new Credential(JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {CredentialProperty.AccessToken, sqlServerToken.AccessToken},
                {CredentialProperty.Expires, sqlServerToken.ExpiresOn.ToString("R")}
            }))
            {
                AuthenticationKind = AuthenticationKind.OAuth2,
                EncryptConnection = true
            };
        }
    }
}