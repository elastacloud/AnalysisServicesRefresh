using System;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using NLog;
using RefreshType = Microsoft.AnalysisServices.Tabular.RefreshType;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class ModelProcessor
    {
        private readonly IDataSourceProcessorFactory _dataSourceProcessorFactory;
        private readonly ILogger _logger;
        private readonly IServerWrapperFactory _serverFactory;
        private readonly ITableProcessor _tableProcessor;
        private readonly ITokenProviderFactory _tokenProviderFactory;

        public ModelProcessor(IServerWrapperFactory serverFactory,
            ITableProcessor tableProcessor,
            IDataSourceProcessorFactory dataSourceProcessorFactory,
            ITokenProviderFactory tokenProviderFactory,
            ILogger logger
        )
        {
            _serverFactory = serverFactory;
            _tableProcessor = tableProcessor;
            _dataSourceProcessorFactory = dataSourceProcessorFactory;
            _tokenProviderFactory = tokenProviderFactory;
            _logger = logger;
        }

        public ModelProcessor()
            : this(
                new ServerWrapperFactory(),
                new TableProcessor(),
                new DataSourceProcessorFactory(),
                new TokenProviderFactory(),
                LogManager.GetCurrentClassLogger()
            )
        {
        }

        public async Task ProcessAsync(ModelConfiguration model)
        {
            try
            {
                _logger.Info("Starting processing.");

                var analysisServicesToken = await GetAnalysisServicesToken(model);

                var connectionString =
                    $"Data Source={model.ServerName};Password={analysisServicesToken.AccessToken};Provider=MSOLAP;";

                using (var server = GetServer(connectionString))
                {
                    var database = GetDatabase(server, model.DatabaseName);

                    _tableProcessor.Process(database, model);
                    await _dataSourceProcessorFactory.Create(model.DataSource.Type)
                        .ProcessAsync(database, model);

                    _logger.Info("Saving model changes.");
                    database.Model.SaveChanges(new SaveOptions {MaxParallelism = 5});

                    _logger.Info("Recalculating model.");
                    database.Model.RequestRefresh(RefreshType.Calculate);
                    database.Model.SaveChanges();

                    _logger.Info("Finished processing.");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        private async Task<Token> GetAnalysisServicesToken(ModelConfiguration model)
        {
            var provider = _tokenProviderFactory.CreateAnalysisServicesTokenProvider(model);
            var token = await provider.CreateAsync();
            _logger.Info("Retrieved Analysis Services authentication token.");

            return token;
        }

        private IServerWrapper GetServer(string connectionString)
        {
            var server = _serverFactory.Create();
            server.Connect(connectionString);
            _logger.Info($"Connected to server {server.Name}.");

            return server;
        }

        private IDatabaseWrapper GetDatabase(IServerWrapper server, string databaseName)
        {
            var database = server.Databases.FindByName(databaseName);

            if (database == null)
            {
                throw new ConnectionException($"Could not connect to database {databaseName}.");
            }

            _logger.Info($"Connected to database {databaseName}.");

            return database;
        }
    }
}