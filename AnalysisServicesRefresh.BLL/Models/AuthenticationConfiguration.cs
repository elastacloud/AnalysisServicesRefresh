using AnalysisServicesRefresh.BLL.KeyVaults;

namespace AnalysisServicesRefresh.BLL.Models
{
    public class AuthenticationConfiguration
    {
        public string AnalysisServicesClientIdName { get; set; }
        public string AnalysisServicesClientSecretName { get; set; }
        public string DirectoryId { get; set; }
        public string KeyVaultClientId { get; set; }
        public string KeyVaultAuthentication { get; set; }
        public string KeyVaultBaseUri { get; set; }
        public AuthenticationType Type { get; set; }
    }
}