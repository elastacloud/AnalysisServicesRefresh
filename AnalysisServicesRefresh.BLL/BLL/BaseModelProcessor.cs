using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RefreshType = Microsoft.AnalysisServices.Tabular.RefreshType;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public abstract class BaseModelProcessor : IModelProcessor
    {
        protected readonly IDataSourceFactory _dataSourceFactory;
        protected readonly ILogger _logger;
        private readonly IRefreshFactory _refreshFactory;
        private readonly IServerWrapperFactory _serverFactory;
        private readonly ITokenProviderFactory _tokenProviderFactory;
        protected IDatabaseWrapper _database;
        private ModelConfiguration _model;
        private IServerWrapper _server;

        public BaseModelProcessor(
            IServerWrapperFactory serverFactory,
            ITokenProviderFactory tokenProviderFactory,
            IRefreshFactory refreshFactory,
            IDataSourceFactory dataSourceFactory,
            ILogger logger)
        {
            _serverFactory = serverFactory;
            _tokenProviderFactory = tokenProviderFactory;
            _refreshFactory = refreshFactory;
            _dataSourceFactory = dataSourceFactory;
            _logger = logger;
        }

        public async Task ProcessAsync(ModelConfiguration model)
        {
            try
            {
                _model = model;

                await BeforeProcessingAsync();

                _logger.Info("Starting processing.");

                await ConnectAsync();
                var refreshPlans = GetRefreshPlans();
                await ProcessRefreshPlansAsync(_model, refreshPlans);
                RecalculateModel();

                _logger.Info("Finished processing.");

                await AfterProcessingAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                Disconnect();
            }
        }

        protected async Task ConnectAsync()
        {
            var analysisServicesToken = await GetAnalysisServicesTokenAsync();

            var connectionString =
                $"Data Source={_model.ServerName};Password={analysisServicesToken.AccessToken};Provider=MSOLAP;";

            _server = GetServer(connectionString);
            _database = GetDatabase(_model.DatabaseName);
        }

        protected void Disconnect()
        {
            _server?.Disconnect();
            _server?.Dispose();
            _server = null;
            _database = null;
        }

        private List<RefreshPlan> GetRefreshPlans()
        {
            return _model.FullTables
                .Select(x => new RefreshPlan
                {
                    Table = GetTable(x.Name),
                    Refresh = _refreshFactory.CreateFull()
                })
                .Concat(
                    _model.PartitionedTables.Select(x => new RefreshPlan
                    {
                        Table = GetTable(x.Name),
                        Refresh = _refreshFactory.CreatePartitioned(x)
                    })
                )
                .ToList();
        }

        private void RecalculateModel()
        {
            _logger.Info("Recalculating model.");
            _database.Model.RequestRefresh(RefreshType.Calculate);
            _database.Model.SaveChanges();
        }

        private async Task<Token> GetAnalysisServicesTokenAsync()
        {
            var provider = _tokenProviderFactory.CreateAnalysisServicesTokenProvider(_model);
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

        private IDatabaseWrapper GetDatabase(string databaseName)
        {
            var database = _server.Databases.FindByName(databaseName);

            if (database == null)
            {
                throw new ConnectionException($"Could not connect to database {databaseName}.");
            }

            _logger.Info($"Connected to database {databaseName}.");

            return database;
        }

        private ITableWrapper GetTable(string tableName)
        {
            var table = _database.Model.Tables.Find(tableName);

            if (table == null)
            {
                throw new ConnectionException($"Could not find table {tableName}.");
            }

            _logger.Info($"Fetched table {tableName}.");

            return table;
        }

        protected abstract Task ProcessRefreshPlansAsync(ModelConfiguration model, List<RefreshPlan> refreshPlans);
        protected abstract Task BeforeProcessingAsync();
        protected abstract Task AfterProcessingAsync();

        protected class RefreshPlan
        {
            public ITableWrapper Table { get; internal set; }
            public IRefresh Refresh { get; internal set; }
        }
    }
}