using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IModelWrapper
    {
        IDataSourceCollectionWrapper DataSources { get; }
        ITableCollectionWrapper Tables { get; }
        void RequestRefresh(RefreshType type);
        void SaveChanges();
        void SaveChanges(SaveOptions saveOptions);
    }
}