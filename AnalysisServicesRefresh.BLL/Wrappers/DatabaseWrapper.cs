using System.Diagnostics.CodeAnalysis;
using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class DatabaseWrapper : IDatabaseWrapper
    {
        private readonly Database _database;

        public DatabaseWrapper(Database database)
        {
            _database = database;
        }

        public IModelWrapper Model => new ModelWrapper(_database.Model);
    }
}