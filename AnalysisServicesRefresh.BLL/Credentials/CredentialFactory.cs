using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Credentials
{
    public class CredentialFactory : ICredentialFactory
    {
        public ICredential Create(AuthenticationConfiguration authentication)
        {
            return new Credential(authentication);
        }
    }
}