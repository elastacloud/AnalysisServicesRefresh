using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class DataSourceCollectionWrapper : IDataSourceCollectionWrapper
    {
        private readonly DataSourceCollection _dataSourceCollection;

        public DataSourceCollectionWrapper(DataSourceCollection dataSourceCollection)
        {
            _dataSourceCollection = dataSourceCollection;
        }

        public IStructuredDataSourceWrapper Find(string name)
        {
            var dataSource = _dataSourceCollection.Find(name);

            if (dataSource is StructuredDataSource structuredDataSource)
            {
                return new StructuredDataSourceWrapper(structuredDataSource);
            }

            throw new InvalidOperationException("DataSource is not supported.");
        }
    }
}