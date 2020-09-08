using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Credentials
{
    public interface ICredentialFactory
    {
        ICredential Create(AuthenticationConfiguration authentication);
    }
}