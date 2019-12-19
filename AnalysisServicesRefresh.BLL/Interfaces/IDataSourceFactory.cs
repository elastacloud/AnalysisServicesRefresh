using AnalysisServicesRefresh.BLL.Enums;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IDataSourceFactory
    {
        IDataSource Create(DataSourceType dataSourceType);
    }
}