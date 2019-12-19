using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using System.Diagnostics.CodeAnalysis;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class StructuredDataSourceWrapper : IStructuredDataSourceWrapper
    {
        private readonly StructuredDataSource _structuredDataSource;

        public StructuredDataSourceWrapper(StructuredDataSource structuredDataSource)
        {
            _structuredDataSource = structuredDataSource;
        }

        public Credential Credential
        {
            get => _structuredDataSource.Credential;
            set => _structuredDataSource.Credential = value;
        }
    }
}