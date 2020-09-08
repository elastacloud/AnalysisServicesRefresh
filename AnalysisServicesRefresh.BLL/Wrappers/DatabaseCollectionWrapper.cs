using System.Diagnostics.CodeAnalysis;
using Microsoft.AnalysisServices.Tabular;

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