namespace AnalysisServicesRefresh.BLL.Models
{
    public class PartitionConfiguration
    {
        public string Name { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public bool Refresh { get; set; }
    }
}