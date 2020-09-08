using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.ConnectionStrings;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class PowerBiConnectionStringTests
    {
        private ModelConfiguration _configuration;
        private Mock<ICredential> _credential;
        private Mock<ICredentialFactory> _credentialFactory;
        private PowerBiConnectionString _sut;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    DirectoryId = "DirectoryId",
                    AnalysisServicesClientIdName = "PowerBiClientId",
                    AnalysisServicesClientSecretName = "PowerBiClientSecret"
                },
                DataSource = new DataSourceConfiguration(),
                DatabaseName = "DatabaseName",
                FullTables = new List<FullTableConfiguration>(),
                PartitionedTables = new List<PartitionedTableConfiguration>(),
                ServerName = "ServerName"
            };

            _credential = new Mock<ICredential>();
            _credential.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ActiveDirectoryClientCredential
                {
                    ClientId = "PowerBiClientIdFromCredential",
                    ClientSecret = "PowerBiClientSecretFromCredential"
                }));

            _credentialFactory = new Mock<ICredentialFactory>();
            _credentialFactory.Setup(x => x.Create(It.IsAny<AuthenticationConfiguration>()))
                .Returns(_credential.Object);

            _sut = new PowerBiConnectionString(_credentialFactory.Object);
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingModelAuthentication()
        {
            await _sut.GetAsync(_configuration);
            _credentialFactory.Verify(x => x.Create(_configuration.Authentication));
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingPowerBiClientId()
        {
            await _sut.GetAsync(_configuration);
            _credential.Verify(x => x.GetAsync("PowerBiClientId", It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingPowerBiClientSecret()
        {
            await _sut.GetAsync(_configuration);
            _credential.Verify(
                x => x.GetAsync(It.IsAny<string>(), "PowerBiClientSecret", It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestReturnsPowerBiConnectionString()
        {
            var actual = await _sut.GetAsync(_configuration);

            Assert.AreEqual(
                "Data Source=ServerName;User ID=app:PowerBiClientIdFromCredential@DirectoryId;Password=PowerBiClientSecretFromCredential;",
                actual);
        }
    }
}