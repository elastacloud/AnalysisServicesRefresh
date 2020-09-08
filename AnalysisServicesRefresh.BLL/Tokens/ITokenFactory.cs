using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Tokens
{
    public interface ITokenFactory
    {
        IToken Create(ActiveDirectoryClientCredential credential, string authority, string resource);
    }
}