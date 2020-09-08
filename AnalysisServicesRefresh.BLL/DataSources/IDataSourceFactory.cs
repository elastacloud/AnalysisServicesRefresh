namespace AnalysisServicesRefresh.BLL.DataSources
{
    public interface IDataSourceFactory
    {
        IDataSource Create(DataSourceType dataSourceType);
    }
}