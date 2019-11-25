using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface ITableProcessor
    {
        void Process(IDatabaseWrapper database, ModelConfiguration model);
    }
}