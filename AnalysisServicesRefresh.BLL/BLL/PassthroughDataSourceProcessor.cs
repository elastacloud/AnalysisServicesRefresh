using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class PassthroughDataSourceProcessor : IDataSourceProcessor
    {
        public Task ProcessAsync(IDatabaseWrapper database, ModelConfiguration model)
        {
            return Task.CompletedTask;
        }
    }
}