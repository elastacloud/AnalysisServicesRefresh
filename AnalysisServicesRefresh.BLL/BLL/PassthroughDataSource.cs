using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class PassthroughDataSource : IDataSource
    {
        public Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model)
        {
            return Task.CompletedTask;
        }
    }
}