namespace AnalysisServicesRefresh.BLL.Wrappers
{
    public class ServerWrapperFactory : IServerWrapperFactory
    {
        public IServerWrapper Create()
        {
            return new ServerWrapper();
        }
    }
}