using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices.Tabular;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class TransactionalModelProcessor : BaseModelProcessor
    {
        public TransactionalModelProcessor() : base(
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
                dataSourceFactory,
                logger)
        {
        }

        protected async override Task ProcessRefreshPlansAsync(ModelConfiguration model, List<RefreshPlan> refreshPlans)
        {
            await _dataSourceFactory.Create(model.DataSource.Type)
                        .ProcessAsync(_database, model);

            refreshPlans.ForEach(x =>
            {
                _logger.Info($"Processing table {x.Table.Name}.");
                x.Refresh.Refresh(x.Table);
            });

            _logger.Info("Saving model changes.");
            _database.Model.SaveChanges(new SaveOptions { MaxParallelism = 5 });
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