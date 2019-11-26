using AnalysisServicesRefresh.BLL.Enums;

namespace AnalysisServicesRefresh.BLL.Models
{
    public class DataSourceConfiguration
    {
        public DataSourceType Type { get; set; }
        public string Name { get; set; }
        public string ClientIdName { get; set; }
        public string ClientSecretName { get; set; }
    }
}