using System;

namespace AnalysisServicesRefresh.BLL.DataSources
{
    public class DataSourceFactory : IDataSourceFactory
    {
        public IDataSource Create(DataSourceType dataSourceType)
        {
            switch (dataSourceType)
            {
                case DataSourceType.OAuth:
                    return new SqlServerOAuthDataSource();
                case DataSourceType.Passthrough:
                    return new PassthroughDataSource();
                default:
                    throw new InvalidOperationException("DataSourceType is unknown.");
            }
        }
    }
}