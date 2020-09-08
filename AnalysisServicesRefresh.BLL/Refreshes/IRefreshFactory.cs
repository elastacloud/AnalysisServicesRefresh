using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Refreshes
{
    public interface IRefreshFactory
    {
        IRefresh CreateFull();
        IRefresh CreatePartitioned(PartitionedTableConfiguration partitionedTable);
    }
}