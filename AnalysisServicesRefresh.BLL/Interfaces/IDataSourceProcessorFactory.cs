using AnalysisServicesRefresh.BLL.Enums;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IDataSourceProcessorFactory
    {
        IDataSourceProcessor Create(DataSourceType dataSourceType);
    }
}