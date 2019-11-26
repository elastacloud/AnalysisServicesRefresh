using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Factories
{
    public class RefreshFactory : IRefreshFactory
    {
        public IRefresh CreateFull()
        {
            return new FullRefresh();
        }

        public IRefresh CreatePartitioned(PartitionedTableConfiguration partitionedTable)
        {
            return new PartitionedRefresh(partitionedTable);
        }
    }
}