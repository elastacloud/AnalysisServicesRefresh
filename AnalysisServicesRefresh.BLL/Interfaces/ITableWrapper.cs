using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface ITableWrapper
    {
        IPartitionCollectionWrapper Partitions { get; }
        string Name { get; }
        void RequestRefresh(RefreshType type);
    }
}