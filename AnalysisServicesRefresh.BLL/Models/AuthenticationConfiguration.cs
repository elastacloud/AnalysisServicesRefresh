using AnalysisServicesRefresh.BLL.KeyVaults;

namespace AnalysisServicesRefresh.BLL.Models
{
    public class AuthenticationConfiguration
    {
        public string AnalysisServicesClientId { get; set; }
        public string AnalysisServicesClientSecret { get; set; }
        public string DirectoryId { get; set; }
        public string KeyVaultClientId { get; set; }
        public string KeyVaultAuthentication { get; set; }
        public KeyVaultAuthenticationType KeyVaultAuthenticationType { get; set; }
        public string KeyVaultBaseUri { get; set; }
    }
}