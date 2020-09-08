using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Wrappers;

namespace AnalysisServicesRefresh.BLL.DataSources
{
    public class PassthroughDataSource : IDataSource
    {
        public Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}