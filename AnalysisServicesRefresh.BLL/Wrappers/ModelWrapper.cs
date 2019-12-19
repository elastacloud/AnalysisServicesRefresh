using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using System.Diagnostics.CodeAnalysis;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class ModelWrapper : IModelWrapper
    {
        private readonly Model _model;

        public ModelWrapper(Model model)
        {
            _model = model;
        }

        public void RequestRefresh(RefreshType type)
        {
            _model.RequestRefresh(type);
        }

        public void SaveChanges()
        {
            _model.SaveChanges();
        }

        public void SaveChanges(SaveOptions saveOptions)
        {
            _model.SaveChanges(saveOptions);
        }

        public IDataSourceCollectionWrapper DataSources => new DataSourceCollectionWrapper(_model.DataSources);
        public ITableCollectionWrapper Tables => new TableCollectionWrapper(_model.Tables);
    }
}