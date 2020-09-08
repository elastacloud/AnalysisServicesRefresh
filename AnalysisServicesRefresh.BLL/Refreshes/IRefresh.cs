using AnalysisServicesRefresh.BLL.Wrappers;

namespace AnalysisServicesRefresh.BLL.Refreshes
{
    public interface IRefresh
    {
        void Refresh(ITableWrapper tableWrapper);
    }
}