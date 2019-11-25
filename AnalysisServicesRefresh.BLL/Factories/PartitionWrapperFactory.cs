using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Wrappers;

namespace AnalysisServicesRefresh.BLL.Factories
{
    public class PartitionWrapperFactory : IPartitionWrapperFactory
    {
        public IPartitionWrapper Create()
        {
            return new PartitionWrapper();
        }
    }
}