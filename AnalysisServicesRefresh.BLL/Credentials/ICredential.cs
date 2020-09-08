using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Credentials
{
    public interface ICredential
    {
        Task<ActiveDirectoryClientCredential> GetAsync(string clientIdName, string clientSecretName,
            CancellationToken cancellationToken = default);
    }
}