using System.Collections.Generic;

namespace AnalysisServicesRefresh.BLL.Models
{
    public class PartitionedTableConfiguration
    {
        public string Name { get; set; }
        public List<PartitionConfiguration> Partitions { get; set; }
    }
}