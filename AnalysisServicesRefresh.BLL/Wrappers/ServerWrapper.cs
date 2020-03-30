using System.Diagnostics.CodeAnalysis;
using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class ServerWrapper : IServerWrapper
    {
        private readonly Server _server;

        public ServerWrapper()
        {
            _server = new Server();
        }

        public void Connect(string connectionString)
        {
            _server.Connect(connectionString);
        }

        public void Disconnect()
        {
            _server.Disconnect();
        }

        public string Name => _server.Name;
        public IDatabaseCollectionWrapper Databases => new DatabaseCollectionWrapper(_server.Databases);

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}