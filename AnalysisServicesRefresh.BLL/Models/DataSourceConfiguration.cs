using AnalysisServicesRefresh.BLL.DataSources;

namespace AnalysisServicesRefresh.BLL.Models
{
    public class DataSourceConfiguration
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public DataSourceType Type { get; set; }
        public string Username { get; set; }
    }
}