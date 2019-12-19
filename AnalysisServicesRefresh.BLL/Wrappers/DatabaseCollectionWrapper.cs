using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using System.Diagnostics.CodeAnalysis;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class DatabaseCollectionWrapper : IDatabaseCollectionWrapper
    {
        private readonly DatabaseCollection _databaseCollection;

        public DatabaseCollectionWrapper(DatabaseCollection databaseCollection)
        {
            _databaseCollection = databaseCollection;
        }

        public IDatabaseWrapper FindByName(string name)
        {
            return new DatabaseWrapper(_databaseCollection.FindByName(name));
        }
    }
}