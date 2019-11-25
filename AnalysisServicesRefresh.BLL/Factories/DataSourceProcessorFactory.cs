using System;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Interfaces;

namespace AnalysisServicesRefresh.BLL.Factories
{
    public class DataSourceProcessorFactory : IDataSourceProcessorFactory
    {
        public IDataSourceProcessor Create(DataSourceType dataSourceType)
        {
            switch (dataSourceType)
            {
                case DataSourceType.OAuth:
                    return new SqlServerOAuthDataSourceProcessor();
                case DataSourceType.Passthrough:
                    return new PassthroughDataSourceProcessor();
                default:
                    throw new InvalidOperationException("DataSourceType is unknown.");
            }
        }
    }
}