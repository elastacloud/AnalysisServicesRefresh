using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IStructuredDataSourceWrapper
    {
        Credential Credential { get; set; }
    }
}