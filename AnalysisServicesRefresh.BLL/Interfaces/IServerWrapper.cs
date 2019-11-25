using System;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IServerWrapper : IDisposable
    {
        string Name { get; }
        IDatabaseCollectionWrapper Databases { get; }
        void Connect(string connectionString);
    }
}