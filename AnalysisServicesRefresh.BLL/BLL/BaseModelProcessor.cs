using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using NLog;
using RefreshType = Microsoft.AnalysisServices.Tabular.RefreshType;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public abstract class BaseModelProcessor : IModelProcessor
    {
        private readonly IRefreshFactory _refreshFactory;
        private readonly IServerWrapperFactory _serverFactory;
        private readonly ITokenProviderFactory _tokenProviderFactory;
        protected readonly ILogger Logger;
        private CancellationToken _cancellationToken;
        private ModelConfiguration _model;

        protected BaseModelProcessor(
            IServerWrapperFactory serverFactory,
            ITokenProviderFactory tokenProviderFactory,
            IRefreshFactory refreshFactory,
            ILogger logger)
        {
            _serverFactory = serverFactory;
            _tokenProviderFactory = tokenProviderFactory;
            _refreshFactory = refreshFactory;
            Logger = logger;
        }

        public async Task ProcessAsync(ModelConfiguration model, CancellationToken cancellationToken = default)
        {
            try
            {
                _model = model;
                _cancellationToken = cancellationToken;

                await BeforeProcessingAsync();

                Logger.Info("Starting processing.");

                var refreshPlans = GetRefreshPlans();
                await ProcessAsync(_model, refreshPlans, cancellationToken);
                await RecalculateModelAsync();

                Logger.Info("Finished processing.");

                await AfterProcessingAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        protected async Task<IServerWrapper> GetServerAsync()
        {
            var analysisServicesToken = await GetAnalysisServicesTokenAsync();

            var connectionString =
                $"Data Source={_model.ServerName};Password={analysisServicesToken.AccessToken};Provider=MSOLAP;";

            var server = _serverFactory.Create();
            server.Connect(connectionString);
            Logger.Info($"Connected to server {server.Name}.");

            return server;
        }

        protected IDatabaseWrapper GetDatabase(IServerWrapper server, string databaseName)
        {
            var database = server.Databases.FindByName(databaseName);

            if (database == null)
            {
                throw new ConnectionException($"Could not connect to database {databaseName}.");
            }

            Logger.Info($"Connected to database {databaseName}.");

            return database;
        }

        protected ITableWrapper GetTable(IDatabaseWrapper database, string tableName)
        {
            var table = database.Model.Tables.Find(tableName);

            if (table == null)
            {
                throw new ConnectionException($"Could not find table {tableName}.");
            }

            Logger.Info($"Fetched table {tableName}.");

            return table;
        }

        private List<RefreshPlan> GetRefreshPlans()
        {
            return _model.FullTables
                .Select(x => new RefreshPlan
                {
                    Table = x.Name,
                    Refresh = _refreshFactory.CreateFull()
                })
                .Concat(
                    _model.PartitionedTables.Select(x => new RefreshPlan
                    {
                        Table = x.Name,
                        Refresh = _refreshFactory.CreatePartitioned(x)
                    })
                )
                .ToList();
        }

        private async Task RecalculateModelAsync()
        {
            using (var server = await GetServerAsync())
            {
                var database = GetDatabase(server, _model.DatabaseName);
                Logger.Info("Recalculating model.");
                database.Model.RequestRefresh(RefreshType.Calculate);
                database.Model.SaveChanges();
                server.Disconnect();
            }
        }

        private async Task<Token> GetAnalysisServicesTokenAsync()
        {
            var provider = _tokenProviderFactory.CreateAnalysisServicesTokenProvider(_model);
            var token = await provider.CreateAsync(_cancellationToken);
            Logger.Info("Retrieved Analysis Services authentication token.");

            return token;
        }

        protected abstract Task ProcessAsync(ModelConfiguration model, List<RefreshPlan> refreshPlans,
            CancellationToken cancellationToken);

        protected abstract Task BeforeProcessingAsync();
        protected abstract Task AfterProcessingAsync();

        protected class RefreshPlan
        {
            public string Table { get; internal set; }
            public IRefresh Refresh { get; internal set; }
        }
    }
}