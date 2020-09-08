using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Tests.Fakes
{
    internal class FakePartitionWrapper : IPartitionWrapper
    {
        public RefreshType RefreshType { get; private set; }
        public string Name { get; set; }
        public PartitionSource Source { get; set; }
        public Partition Partition { get; }

        public void RequestRefresh(RefreshType type)
        {
            RefreshType = type;
        }

        public void CopyTo(IPartitionWrapper partition)
        {
            switch (Source)
            {
                case MPartitionSource ms:
                    partition.Source = new MPartitionSource
                    {
                        Expression = ms.Expression
                    };
                    break;
            }
        }
    }
}