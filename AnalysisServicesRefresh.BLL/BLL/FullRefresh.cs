using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using NLog;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class FullRefresh : IRefresh
    {
        private readonly ILogger _logger;

        public FullRefresh() : this(LogManager.GetCurrentClassLogger())
        {
        }

        public FullRefresh(ILogger logger)
        {
            _logger = logger;
        }

        public void Refresh(ITableWrapper table)
        {
            _logger.Info($"Requesting full refresh for table {table.Name}.");
            table.RequestRefresh(RefreshType.Full);
        }
    }
}