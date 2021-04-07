using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.DataSources;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using Credential = Microsoft.AnalysisServices.Tabular.Credential;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class UsernamePasswordDataSourceTests
    {
        private ModelConfiguration _configuration;
        private Mock<ICredential> _credential;
        private Mock<ICredentialFactory> _credentialFactory;
        private Mock<IDatabaseWrapper> _database;
        private Mock<IStructuredDataSourceWrapper> _dataSource;
        private Mock<IDataSourceCollectionWrapper> _dataSourceCollection;
        private Mock<ILogger> _logger;
        private Mock<IModelWrapper> _model;
        private UsernamePasswordDataSource _sut;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    DirectoryId = "DirectoryId"
                },
                DataSource = new DataSourceConfiguration
                {
                    Name = "DataSourceName",
                    Username = "DataSourceClientId",
                    Password = "DataSourceClientSecret"
                },
                DatabaseName = "DatabaseName",
                FullTables = new List<FullTableConfiguration>(),
                PartitionedTables = new List<PartitionedTableConfiguration>(),
                ServerName = "ServerName"
            };

            _credential = new Mock<ICredential>();
            _credential.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ClientCredential
                {
                    Username = "DataSourceClientIdFromCredential",
                    Password = "DataSourceClientSecretFromCredential"
                }));

            _credentialFactory = new Mock<ICredentialFactory>();
            _credentialFactory.Setup(x => x.Create(It.IsAny<AuthenticationConfiguration>()))
                .Returns(_credential.Object);

            _logger = new Mock<ILogger>();

            _dataSource = new Mock<IStructuredDataSourceWrapper>();

            _dataSourceCollection = new Mock<IDataSourceCollectionWrapper>();
            _dataSourceCollection.Setup(x => x.Find(It.IsAny<string>())).Returns(_dataSource.Object);

            _model = new Mock<IModelWrapper>();
            _model.Setup(x => x.DataSources).Returns(_dataSourceCollection.Object);

            _database = new Mock<IDatabaseWrapper>();
            _database.Setup(x => x.Model).Returns(_model.Object);

            _sut = new UsernamePasswordDataSource(_credentialFactory.Object, _logger.Object);
        }

        [TestMethod]
        public async Task TestFindsDataSourceInDatabase()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _dataSourceCollection.Verify(x => x.Find(_configuration.DataSource.Name));
        }

        [TestMethod]
        public async Task TestInvalidDataSourceInDatabaseThrowsConnectionException()
        {
            _dataSourceCollection.Setup(x => x.Find(It.IsAny<string>())).Returns((IStructuredDataSourceWrapper) null);
            await Assert.ThrowsExceptionAsync<ConnectionException>(() =>
                _sut.ProcessAsync(_database.Object, _configuration));
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingModelAuthentication()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _credentialFactory.Verify(x => x.Create(_configuration.Authentication));
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingDataSourceClientId()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _credential.Verify(x =>
                x.GetAsync("DataSourceClientId", It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingDataSourceClientSecret()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _credential.Verify(x =>
                x.GetAsync(It.IsAny<string>(), "DataSourceClientSecret", It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestAssignsCredentialToDataSourceWithUsernamePassword()
        {
            Credential credential = null;
            _dataSource.SetupSet(x => x.Credential = It.IsAny<Credential>()).Callback<Credential>(x => credential = x);
            await _sut.ProcessAsync(_database.Object, _configuration);

            Assert.AreEqual(AuthenticationKind.UsernamePassword, credential?.AuthenticationKind);
        }

        [TestMethod]
        public async Task TestAssignsCredentialToDataSourceWithClientSecretFromCredential()
        {
            Credential credential = null;
            _dataSource.SetupSet(x => x.Credential = It.IsAny<Credential>()).Callback<Credential>(x => credential = x);
            await _sut.ProcessAsync(_database.Object, _configuration);

            Assert.AreEqual("DataSourceClientSecretFromCredential", credential.Password);
        }

        [TestMethod]
        public async Task TestAssignsCredentialToDataSourceWithClientIdFromCredential()
        {
            Credential credential = null;
            _dataSource.SetupSet(x => x.Credential = It.IsAny<Credential>()).Callback<Credential>(x => credential = x);
            await _sut.ProcessAsync(_database.Object, _configuration);

            Assert.AreEqual("DataSourceClientIdFromCredential", credential.Username);
        }

        [TestMethod]
        public async Task TestLogsFetchDataSource()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _logger.Verify(x => x.Info($"Fetched data source {_configuration.DataSource.Name}."));
        }
    }
}