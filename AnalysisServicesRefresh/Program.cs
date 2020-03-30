using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AnalysisServicesRefresh
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var json = File.ReadAllText("activity.json");

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> {new StringEnumConverter()}
            };

            var properties = JsonConvert.DeserializeObject<ActivityProperties>(json, settings)
                .TypeProperties.ExtendedProperties;

            var model = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    AnalysisServicesClientIdName = properties.AnalysisServicesClientIdName,
                    AnalysisServicesClientSecretName = properties.AnalysisServicesClientSecretName,
                    Type = properties.AuthenticationType,
                    DirectoryId = properties.DirectoryId,
                    KeyVaultAuthentication = properties.KeyVaultAuthentication,
                    KeyVaultClientId = properties.KeyVaultClientId,
                    KeyVaultBaseUri = properties.KeyVaultBaseUri
                },
                DataSource = new DataSourceConfiguration
                {
                    ClientIdName = properties.DataSourceClientIdName,
                    ClientSecretName = properties.DataSourceClientSecretName,
                    Name = properties.DataSourceName,
                    Type = properties.DataSourceType
                },
                DatabaseName = properties.DatabaseName,
                FullTables = properties.FullTables,
                ModelProcessorType = properties.ModelProcessorType,
                PartitionedTables = properties.PartitionedTables,
                ServerName = properties.ServerName
            };

            await new ModelProcessorFactory().Create(model.ModelProcessorType).ProcessAsync(model);
        }
    }
}