using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices.Tabular;
using NLog;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class TransactionalModelProcessor : BaseModelProcessor
    {
        private readonly IDataSourceFactory _dataSourceFactory;

        public TransactionalModelProcessor() : this(
            new ServerWrapperFactory(),
            new TokenProviderFactory(),
            new RefreshFactory(),
            new DataSourceFactory(),
            LogManager.GetCurrentClassLogger())
        {
        }

        public TransactionalModelProcessor(
            IServerWrapperFactory serverFactory,
            ITokenProviderFactory tokenProviderFactory,
            IRefreshFactory refreshFactory,
            IDataSourceFactory dataSourceFactory,
            ILogger logger) : base(
            serverFactory,
            tokenProviderFactory,
            refreshFactory,
            logger)
        {
            _dataSourceFactory = dataSourceFactory;
        }

        protected override async Task ProcessAsync(ModelConfiguration model, List<RefreshPlan> refreshPlans,
            CancellationToken cancellationToken)
        {
            using (var server = await GetServerAsync())
            {
                try
                {
                    var database = GetDatabase(server, model.DatabaseName);

                    await _dataSourceFactory.Create(model.DataSource.Type)
                        .ProcessAsync(database, model, cancellationToken);

                    refreshPlans.ForEach(x =>
                    {
                        Logger.Info($"Processing table {x.Table}.");
                        var table = GetTable(database, x.Table);
                        x.Refresh.Refresh(table);
                    });

                    Logger.Info("Saving model changes.");
                    database.Model.SaveChanges(new SaveOptions {MaxParallelism = 5});
                }
                finally
                {
                    server.Disconnect();
                }
            }
        }

        protected override Task BeforeProcessingAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task AfterProcessingAsync()
        {
            return Task.CompletedTask;
        }
    }
}