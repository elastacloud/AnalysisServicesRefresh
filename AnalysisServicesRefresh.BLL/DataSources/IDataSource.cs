using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Wrappers;

namespace AnalysisServicesRefresh.BLL.DataSources
{
    public interface IDataSource
    {
        Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model,
            CancellationToken cancellationToken = default);
    }
}