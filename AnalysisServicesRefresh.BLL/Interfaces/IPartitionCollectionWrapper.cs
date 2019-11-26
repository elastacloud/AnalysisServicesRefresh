using System.Collections.Generic;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IPartitionCollectionWrapper : IEnumerable<IPartitionWrapper>
    {
        IPartitionWrapper Find(string name);
        void Add(IPartitionWrapper item);
        bool Remove(IPartitionWrapper item);
    }
}