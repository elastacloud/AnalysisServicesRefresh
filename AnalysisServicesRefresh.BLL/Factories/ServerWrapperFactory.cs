using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Wrappers;

namespace AnalysisServicesRefresh.BLL.Factories
{
    public class ServerWrapperFactory : IServerWrapperFactory
    {
        public IServerWrapper Create()
        {
            return new ServerWrapper();
        }
    }
}