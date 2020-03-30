using System.Diagnostics.CodeAnalysis;
using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class PartitionWrapper : IPartitionWrapper
    {
        public PartitionWrapper(Partition partition)
        {
            Partition = partition;
        }

        public PartitionWrapper() : this(new Partition())
        {
        }

        public Partition Partition { get; }

        public string Name
        {
            get => Partition.Name;
            set => Partition.Name = value;
        }

        public PartitionSource Source
        {
            get => Partition.Source;
            set => Partition.Source = value;
        }

        public void RequestRefresh(RefreshType type)
        {
            Partition.RequestRefresh(type);
        }

        public void CopyTo(IPartitionWrapper partition)
        {
            Partition.CopyTo(partition.Partition);
        }
    }
}