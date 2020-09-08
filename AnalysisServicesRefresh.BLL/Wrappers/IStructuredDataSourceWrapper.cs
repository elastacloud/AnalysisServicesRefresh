using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    public interface IStructuredDataSourceWrapper
    {
        Credential Credential { get; set; }
    }
}