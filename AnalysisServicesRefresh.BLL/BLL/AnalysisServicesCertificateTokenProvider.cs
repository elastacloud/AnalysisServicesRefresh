using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class AnalysisServicesCertificateTokenProvider : BaseCertificateTokenProvider
    {
        public AnalysisServicesCertificateTokenProvider(ModelConfiguration model) : base(model)
        {
        }

        protected override string Authority => $"https://login.microsoftonline.com/{Model.Authentication.DirectoryId}";
        protected override string Resource => "https://*.asazure.windows.net/";
        protected override string ClientIdName => Model.Authentication.AnalysisServicesClientIdName;
        protected override string ClientSecretName => Model.Authentication.AnalysisServicesClientSecretName;
    }
}