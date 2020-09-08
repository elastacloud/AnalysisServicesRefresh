using System;

namespace AnalysisServicesRefresh.BLL.ConnectionStrings
{
    public class ConnectionStringFactory : IConnectionStringFactory
    {
        public IConnectionString Create(string serverName)
        {
            if (serverName.StartsWith("asazure://"))
            {
                return new AnalysisServicesConnectionString();
            }

            if (serverName.StartsWith("powerbi://"))
            {
                return new PowerBiConnectionString();
            }

            throw new InvalidOperationException("Connection string is not supported.");
        }
    }
}