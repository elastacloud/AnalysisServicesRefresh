using System.Collections.Generic;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    public interface IPartitionCollectionWrapper : IEnumerable<IPartitionWrapper>
    {
        IPartitionWrapper Find(string name);
        void Add(IPartitionWrapper item);
        bool Remove(IPartitionWrapper item);
    }
}