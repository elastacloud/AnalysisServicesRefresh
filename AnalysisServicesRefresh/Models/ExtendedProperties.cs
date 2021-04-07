using System.Collections.Generic;
using AnalysisServicesRefresh.BLL.DataSources;
using AnalysisServicesRefresh.BLL.KeyVaults;
using AnalysisServicesRefresh.BLL.ModelProcessors;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.Models
{
    internal class ExtendedProperties
    {
        public string AnalysisServicesClientId { get; set; }
        public string AnalysisServicesClientSecret { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourcePassword { get; set; }
        public DataSourceType DataSourceType { get; set; }
        public string DataSourceUsername { get; set; }
        public string DatabaseName { get; set; }
        public string DirectoryId { get; set; }
        public List<FullTableConfiguration> FullTables { get; set; }
        public string KeyVaultAuthentication { get; set; }
        public KeyVaultAuthenticationType KeyVaultAuthenticationType { get; set; }
        public string KeyVaultBaseUri { get; set; }
        public string KeyVaultClientId { get; set; }
        public int MaxParallelism { get; set; }
        public ModelProcessorType ModelProcessorType { get; set; }
        public List<PartitionedTableConfiguration> PartitionedTables { get; set; }
        public string ServerName { get; set; }
    }
}