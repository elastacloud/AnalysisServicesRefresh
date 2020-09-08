using System.Threading;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.Tokens
{
    public interface IToken
    {
        Task<Models.Token> GetAsync(CancellationToken cancellationToken = default);
    }
}