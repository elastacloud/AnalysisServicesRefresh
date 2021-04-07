using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Credentials
{
    public interface ICredential
    {
        Task<ClientCredential> GetAsync(string username, string password,
            CancellationToken cancellationToken = default);
    }
}