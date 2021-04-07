using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.ConnectionStrings;
using AnalysisServicesRefresh.BLL.DataSources;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Refreshes;
using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.AnalysisServices.Tabular;
using NLog;

namespace AnalysisServicesRefresh.BLL.ModelProcessors
{
    public class NonTransactionalModelProcessor : BaseModelProcessor
    {
        private const int Attempts = 2;
        private readonly IDataSourceFactory _dataSourceFactory;
        private readonly List<string> _failures = new List<string>();

        public NonTransactionalModelProcessor() : this(
            new ServerWrapperFactory(),
            new ConnectionStringFactory(),
            new RefreshFactory(),
            new DataSourceFactory(),
            LogManager.GetCurrentClassLogger())
        {
        }

        public NonTransactionalModelProcessor(
            IServerWrapperFactory serverFactory,
            IConnectionStringFactory connectionStringFactory,
            IRefreshFactory refreshFactory,
            IDataSourceFactory dataSourceFactory,
            ILogger logger) : base(
            serverFactory,
            connectionStringFactory,
            refreshFactory,
            logger)
        {
            _dataSourceFactory = dataSourceFactory;
        }

        protected override async Task ProcessAsync(ModelConfiguration model, List<RefreshPlan> refreshPlans,
            CancellationToken cancellationToken)
        {
            foreach (var refreshPlan in refreshPlans)
            {
                await RetryProcessRefreshPlanAsync(model, refreshPlan, cancellationToken);
            }
        }

        private async Task RetryProcessRefreshPlanAsync(ModelConfiguration model, RefreshPlan refreshPlan,
            CancellationToken cancellationToken)
        {
            var attempt = 0;
            do
            {
                attempt++;

                try
                {
                    await ProcessRefreshPlanAsync(model, refreshPlan, cancellationToken);
                    break;
                }
                catch
                {
                    if (attempt == Attempts)
                    {
                        _failures.Add(refreshPlan.Table);
                    }
                }
            } while (attempt < Attempts);
        }

        private async Task ProcessRefreshPlanAsync(ModelConfiguration model, RefreshPlan refreshPlan,
            CancellationToken cancellationToken)
        {
            try
            {
                using (var server = await GetServerAsync())
                {
                    try
                    {
                        var database = GetDatabase(server, model.DatabaseName);

                        await _dataSourceFactory.Create(model.DataSource.Type)
                            .ProcessAsync(database, model, cancellationToken);

                        Logger.Info($"Processing table {refreshPlan.Table}.");

                        var table = GetTable(database, refreshPlan.Table);
                        refreshPlan.Refresh.Refresh(table);

                        Logger.Info($"Saving {refreshPlan.Table}.");
                        database.Model.SaveChanges(new SaveOptions {MaxParallelism = model.MaxParallelism});
                    }
                    finally
                    {
                        server.Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        protected override Task BeforeProcessingAsync()
        {
            _failures.Clear();
            return Task.CompletedTask;
        }

        protected override Task AfterProcessingAsync()
        {
            if (_failures.Any())
            {
                throw new ApplicationException(
                    $"Non-transactional processing failed for the following tables: {string.Join(", ", _failures)}.");
            }

            return Task.CompletedTask;
        }
    }
}