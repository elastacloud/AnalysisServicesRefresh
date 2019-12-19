using AnalysisServicesRefresh.BLL.Models;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IModelProcessor
    {
        Task ProcessAsync(ModelConfiguration model);
    }
}