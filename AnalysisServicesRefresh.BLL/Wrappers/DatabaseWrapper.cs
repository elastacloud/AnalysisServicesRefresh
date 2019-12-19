using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using System.Diagnostics.CodeAnalysis;

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