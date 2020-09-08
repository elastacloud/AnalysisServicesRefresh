namespace AnalysisServicesRefresh.BLL.Wrappers
{
    public interface IDatabaseCollectionWrapper
    {
        IDatabaseWrapper FindByName(string name);
    }
}