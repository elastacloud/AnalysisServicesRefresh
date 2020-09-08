using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public interface IKeyVault
    {
        Task<ActiveDirectoryClientCredential> GetAsync(string clientIdName, string clientSecretName,
            CancellationToken cancellationToken = default);
    }
}