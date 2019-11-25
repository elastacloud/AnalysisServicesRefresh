using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface ITokenProviderFactory
    {
        ITokenProvider CreateAnalysisServicesTokenProvider(ModelConfiguration model);
        ITokenProvider CreateSqlServerTokenProvider(ModelConfiguration model);
    }
}