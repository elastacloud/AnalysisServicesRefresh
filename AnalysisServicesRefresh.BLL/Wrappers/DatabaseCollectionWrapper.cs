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
            var database = _databaseCollection.FindByName(name);
            return database == null ? null : new DatabaseWrapper(database);
        }
    }
}