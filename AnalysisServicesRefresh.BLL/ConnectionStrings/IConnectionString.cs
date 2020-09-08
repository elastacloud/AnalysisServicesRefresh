using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.ConnectionStrings
{
    public interface IConnectionString
    {
        Task<string> GetAsync(ModelConfiguration model, CancellationToken cancellationToken = default);
    }
}