using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface ITokenProvider
    {
        Task<Token> CreateAsync(CancellationToken cancellationToken = default);
    }
}