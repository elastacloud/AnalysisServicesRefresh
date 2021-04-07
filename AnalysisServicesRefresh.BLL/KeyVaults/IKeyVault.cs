using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public interface IKeyVault
    {
        Task<ClientCredential> GetAsync(string username, string password,
            CancellationToken cancellationToken = default);
    }
}