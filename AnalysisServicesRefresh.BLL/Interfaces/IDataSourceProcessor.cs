using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IDataSourceProcessor
    {
        Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model);
    }
}