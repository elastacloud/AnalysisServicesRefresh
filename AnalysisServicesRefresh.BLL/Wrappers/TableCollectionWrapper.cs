using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using System.Diagnostics.CodeAnalysis;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class TableCollectionWrapper : ITableCollectionWrapper
    {
        private readonly TableCollection _tableCollection;

        public TableCollectionWrapper(TableCollection tableCollection)
        {
            _tableCollection = tableCollection;
        }

        public ITableWrapper Find(string name)
        {
            var table = _tableCollection.Find(name);
            return table == null ? null : new TableWrapper(table);
        }
    }
}