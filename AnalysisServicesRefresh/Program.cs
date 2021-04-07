using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.ModelProcessors;
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
                    AnalysisServicesClientId = properties.AnalysisServicesClientId,
                    AnalysisServicesClientSecret = properties.AnalysisServicesClientSecret,
                    KeyVaultAuthenticationType = properties.KeyVaultAuthenticationType,
                    DirectoryId = properties.DirectoryId,
                    KeyVaultAuthentication = properties.KeyVaultAuthentication,
                    KeyVaultClientId = properties.KeyVaultClientId,
                    KeyVaultBaseUri = properties.KeyVaultBaseUri
                },
                DataSource = new DataSourceConfiguration
                {
                    Username = properties.DataSourceUsername,
                    Password = properties.DataSourcePassword,
                    Name = properties.DataSourceName,
                    Type = properties.DataSourceType
                },
                DatabaseName = properties.DatabaseName,
                FullTables = properties.FullTables,
                MaxParallelism = properties.MaxParallelism,
                ModelProcessorType = properties.ModelProcessorType,
                PartitionedTables = properties.PartitionedTables,
                ServerName = properties.ServerName
            };

            await new ModelProcessorFactory().Create(model.ModelProcessorType).ProcessAsync(model);
        }
    }
}