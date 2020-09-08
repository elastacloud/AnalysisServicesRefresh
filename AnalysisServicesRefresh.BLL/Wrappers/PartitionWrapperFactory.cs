namespace AnalysisServicesRefresh.BLL.Wrappers
{
    public class PartitionWrapperFactory : IPartitionWrapperFactory
    {
        public IPartitionWrapper Create()
        {
            return new PartitionWrapper();
        }
    }
}