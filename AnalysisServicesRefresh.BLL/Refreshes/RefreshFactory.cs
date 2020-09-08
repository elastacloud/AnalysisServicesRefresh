using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Refreshes
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