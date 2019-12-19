using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class SqlServerOAuthDataSourceTests
    {
        private ModelConfiguration _configuration;
        private Mock<IDatabaseWrapper> _database;
        private Mock<IStructuredDataSourceWrapper> _dataSource;
        private Mock<IDataSourceCollectionWrapper> _dataSourceCollection;
        private Mock<ILogger> _logger;
        private Mock<IModelWrapper> _model;
        private SqlServerOAuthDataSource _sut;
        private Mock<ITokenProvider> _tokenProvider;
        private Mock<ITokenProviderFactory> _tokenProviderFactory;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration(),
                DataSource = new DataSourceConfiguration
                {
                    Name = "DataSourceName"
                },
                DatabaseName = "ModelDatabaseName",

                FullTables = new List<FullTableConfiguration>(),
                PartitionedTables = new List<PartitionedTableConfiguration>(),
                ServerName = "ModelConnectionString"
            };

            _tokenProvider = new Mock<ITokenProvider>();
            _tokenProvider.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Token
                {
                    AccessToken = "SqlServerToken",
                    ExpiresOn = new DateTimeOffset(2019, 10, 24, 12, 0, 0, TimeSpan.Zero)
                }));

            _tokenProviderFactory = new Mock<ITokenProviderFactory>();
            _tokenProviderFactory.Setup(x => x.CreateSqlServerTokenProvider(It.IsAny<ModelConfiguration>()))
                .Returns(_tokenProvider.Object);

            _logger = new Mock<ILogger>();

            _dataSource = new Mock<IStructuredDataSourceWrapper>();

            _dataSourceCollection = new Mock<IDataSourceCollectionWrapper>();
            _dataSourceCollection.Setup(x => x.Find(It.IsAny<string>())).Returns(_dataSource.Object);

            _model = new Mock<IModelWrapper>();
            _model.Setup(x => x.DataSources).Returns(_dataSourceCollection.Object);

            _database = new Mock<IDatabaseWrapper>();
            _database.Setup(x => x.Model).Returns(_model.Object);

            _sut = new SqlServerOAuthDataSource(_tokenProviderFactory.Object, _logger.Object);
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
            _dataSourceCollection.Setup(x => x.Find(It.IsAny<string>())).Returns((IStructuredDataSourceWrapper)null);
            await Assert.ThrowsExceptionAsync<ConnectionException>(() =>
                _sut.ProcessAsync(_database.Object, _configuration));
        }

        [TestMethod]
        public async Task TestGetsSqlServerTokenUsingModel()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _tokenProviderFactory.Verify(x => x.CreateSqlServerTokenProvider(_configuration));
        }

        [TestMethod]
        public async Task TestAssignsCredentialToDataSourceWithOAuth2()
        {
            Credential credential = null;
            _dataSource.SetupSet(x => x.Credential = It.IsAny<Credential>()).Callback<Credential>(x => credential = x);
            await _sut.ProcessAsync(_database.Object, _configuration);

            Assert.AreEqual(AuthenticationKind.OAuth2, credential?.AuthenticationKind);
        }

        [TestMethod]
        public async Task TestAssignsCredentialToDataSourceWithEncryptConnection()
        {
            Credential credential = null;
            _dataSource.SetupSet(x => x.Credential = It.IsAny<Credential>()).Callback<Credential>(x => credential = x);
            await _sut.ProcessAsync(_database.Object, _configuration);

            Assert.IsTrue(credential.EncryptConnection);
        }

        [TestMethod]
        public async Task TestAssignsCredentialToDataSourceWithAccessToken()
        {
            Credential credential = null;
            _dataSource.SetupSet(x => x.Credential = It.IsAny<Credential>()).Callback<Credential>(x => credential = x);
            await _sut.ProcessAsync(_database.Object, _configuration);

            Assert.AreEqual("SqlServerToken", credential[CredentialProperty.AccessToken]);
        }

        [TestMethod]
        public async Task TestAssignsCredentialToDataSourceWithExpiry()
        {
            Credential credential = null;
            _dataSource.SetupSet(x => x.Credential = It.IsAny<Credential>()).Callback<Credential>(x => credential = x);
            await _sut.ProcessAsync(_database.Object, _configuration);

            Assert.AreEqual(
                new DateTimeOffset(2019, 10, 24, 12, 0, 0, TimeSpan.Zero).ToString("R"),
                credential[CredentialProperty.Expires]
            );
        }

        [TestMethod]
        public async Task TestLogsFetchDataSource()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _logger.Verify(x => x.Info($"Fetched data source {_configuration.DataSource.Name}."));
        }

        [TestMethod]
        public async Task TestLogsGetSqlServerAuthenticationToken()
        {
            await _sut.ProcessAsync(_database.Object, _configuration);
            _logger.Verify(x => x.Info("Retrieved SQL Server authentication token."));
        }
    }
}