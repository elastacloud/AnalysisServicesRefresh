using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.ModelProcessors
{
    public interface IModelProcessor
    {
        Task ProcessAsync(ModelConfiguration model, CancellationToken cancellationToken = default);
    }
}