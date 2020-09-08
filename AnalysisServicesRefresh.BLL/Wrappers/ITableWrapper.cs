using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    public interface ITableWrapper
    {
        IPartitionCollectionWrapper Partitions { get; }
        string Name { get; }
        void RequestRefresh(RefreshType type);
    }
}