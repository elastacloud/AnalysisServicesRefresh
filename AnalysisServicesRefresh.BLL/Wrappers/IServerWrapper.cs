using System;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    public interface IServerWrapper : IDisposable
    {
        string Name { get; }
        IDatabaseCollectionWrapper Databases { get; }
        void Connect(string connectionString);
        void Disconnect();
    }
}