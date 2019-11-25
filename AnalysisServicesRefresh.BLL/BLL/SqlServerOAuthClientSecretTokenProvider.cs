using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class SqlServerOAuthClientSecretTokenProvider : BaseClientSecretTokenProvider
    {
        public SqlServerOAuthClientSecretTokenProvider(ModelConfiguration model) : base(model)
        {
        }

        protected override string Authority => $"https://login.windows.net/{Model.Authentication.DirectoryId}";
        protected override string Resource => "https://database.windows.net/";
        protected override string ClientIdName => Model.DataSource.ClientIdName;
        protected override string ClientSecretName => Model.DataSource.ClientSecretName;
    }
}