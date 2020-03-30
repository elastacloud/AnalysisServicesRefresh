using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.BLL
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