using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IDataSource
    {
        Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model,
            CancellationToken cancellationToken = default);
    }
}