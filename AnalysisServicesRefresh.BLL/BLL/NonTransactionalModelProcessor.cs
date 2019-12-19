using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices.Tabular;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class NonTransactionalModelProcessor : BaseModelProcessor
    {
        private readonly List<string> _failures = new List<string>();

        public NonTransactionalModelProcessor() : base(
            new ServerWrapperFactory(),
            new TokenProviderFactory(),
            new RefreshFactory(),
            new DataSourceFactory(),
            LogManager.GetCurrentClassLogger())
        {
        }

        public NonTransactionalModelProcessor(
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

        protected override async Task ProcessRefreshPlansAsync(ModelConfiguration model, List<RefreshPlan> refreshPlans)
        {
            foreach (var plan in refreshPlans)
            {
                try
                {
                    await _dataSourceFactory.Create(model.DataSource.Type)
                        .ProcessAsync(_database, model);

                    _logger.Info($"Processing table {plan.Table.Name}.");
                    plan.Refresh.Refresh(plan.Table);

                    _logger.Info($"Saving {plan.Table.Name}.");
                    _database.Model.SaveChanges(new SaveOptions { MaxParallelism = 5 });
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    _failures.Add(plan.Table.Name);

                    Disconnect();
                    await ConnectAsync();
                }
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
                throw new ApplicationException($"Non-transactional processing failed for the following tables: {string.Join(", ", _failures)}.");
            }

            return Task.CompletedTask;
        }
    }
}