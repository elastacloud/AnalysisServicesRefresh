using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IRefreshFactory
    {
        IRefresh CreateFull();
        IRefresh CreatePartitioned(PartitionedTableConfiguration partitionedTable);
    }
}