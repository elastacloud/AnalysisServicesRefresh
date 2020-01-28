using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class PartitionCollectionWrapper : IPartitionCollectionWrapper
    {
        private readonly PartitionCollection _partitionCollection;

        public PartitionCollectionWrapper(PartitionCollection partitionCollection)
        {
            _partitionCollection = partitionCollection;
        }

        public IPartitionWrapper Find(string name)
        {
            var partition = _partitionCollection.Find(name);
            return partition == null ? null : new PartitionWrapper(partition);
        }

        public void Add(IPartitionWrapper item)
        {
            _partitionCollection.Add(item.Partition);
        }

        public bool Remove(IPartitionWrapper item)
        {
            return _partitionCollection.Remove(item.Partition);
        }

        public IEnumerator<IPartitionWrapper> GetEnumerator()
        {
            return _partitionCollection.Select(x => new PartitionWrapper(x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}