using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class SqlServerOAuthDataSource : IDataSource
    {
        private readonly ILogger _logger;
        private readonly ITokenProviderFactory _tokenProviderFactory;

        public SqlServerOAuthDataSource() : this(new TokenProviderFactory(),
            LogManager.GetCurrentClassLogger())
        {
        }

        public SqlServerOAuthDataSource(ITokenProviderFactory tokenProviderFactory, ILogger logger)
        {
            _tokenProviderFactory = tokenProviderFactory;
            _logger = logger;
        }

        public async Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model)
        {
            var dataSource = GetDataSource(database, model.DataSource.Name);
            var sqlServerToken = await GetSqlServerToken(model);
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

        private async Task<Token> GetSqlServerToken(ModelConfiguration model)
        {
            var provider = _tokenProviderFactory.CreateSqlServerTokenProvider(model);
            var token = await provider.CreateAsync();
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