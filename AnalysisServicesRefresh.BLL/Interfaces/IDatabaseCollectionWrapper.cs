namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IDatabaseCollectionWrapper
    {
        IDatabaseWrapper FindByName(string name);
    }
}