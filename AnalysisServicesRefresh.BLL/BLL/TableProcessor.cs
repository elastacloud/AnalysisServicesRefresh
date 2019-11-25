using System.Linq;
using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using NLog;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class TableProcessor : ITableProcessor
    {
        private readonly ILogger _logger;
        private readonly IRefreshFactory _refreshFactory;

        public TableProcessor() : this(new RefreshFactory(), LogManager.GetCurrentClassLogger())
        {
        }

        public TableProcessor(IRefreshFactory refreshFactory, ILogger logger)
        {
            _refreshFactory = refreshFactory;
            _logger = logger;
        }

        public void Process(IDatabaseWrapper database, ModelConfiguration model)
        {
            model.FullTables
                .Select(x => new
                    {
                        Table = GetTable(database, x.Name),
                        Refresh = _refreshFactory.CreateFull(),
                        x.Name
                    }
                )
                .Concat(
                    model.PartitionedTables.Select(x => new
                    {
                        Table = GetTable(database, x.Name),
                        Refresh = _refreshFactory.CreatePartitioned(x),
                        x.Name
                    })
                )
                .ToList()
                .ForEach(x =>
                {
                    _logger.Info($"Processing table {x.Name}.");
                    x.Refresh.Refresh(x.Table);
                });
        }

        private ITableWrapper GetTable(IDatabaseWrapper database, string tableName)
        {
            var table = database.Model.Tables.Find(tableName);

            if (table == null)
            {
                throw new ConnectionException($"Could not find table {tableName}.");
            }

            _logger.Info($"Fetched table {tableName}.");

            return table;
        }
    }
}