namespace AnalysisServicesRefresh.BLL.ConnectionStrings
{
    public interface IConnectionStringFactory
    {
        IConnectionString Create(string serverName);
    }
}