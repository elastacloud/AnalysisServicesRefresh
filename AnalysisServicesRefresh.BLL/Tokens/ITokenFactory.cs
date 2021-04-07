using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Tokens
{
    public interface ITokenFactory
    {
        IToken Create(ClientCredential credential, string authority, string resource);
    }
}