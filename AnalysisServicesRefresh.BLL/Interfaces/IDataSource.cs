using AnalysisServicesRefresh.BLL.Models;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IDataSource
    {
        Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model);
    }
}