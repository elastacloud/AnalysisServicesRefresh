using System;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Interfaces;

namespace AnalysisServicesRefresh.BLL.Factories
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