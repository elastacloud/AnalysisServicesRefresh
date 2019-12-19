using AnalysisServicesRefresh.BLL.Enums;
using System.Collections.Generic;

namespace AnalysisServicesRefresh.BLL.Models
{
    public class ModelConfiguration
    {
        public AuthenticationConfiguration Authentication { get; set; }
        public DataSourceConfiguration DataSource { get; set; }
        public string DatabaseName { get; set; }
        public List<FullTableConfiguration> FullTables { get; set; }
        public List<PartitionedTableConfiguration> PartitionedTables { get; set; }
        public string ServerName { get; set; }
        public ModelProcessorType ModelProcessorType { get; set; }
    }
}