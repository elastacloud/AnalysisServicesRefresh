using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using DataSourceType = AnalysisServicesRefresh.BLL.Enums.DataSourceType;
using Partition = AnalysisServicesRefresh.BLL.Models.PartitionConfiguration;
using RefreshType = Microsoft.AnalysisServices.Tabular.RefreshType;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class ModelProcessorTests
    {
        private ModelConfiguration _configuration;
        private Mock<IDatabaseWrapper> _database;
        private Mock<IDatabaseCollectionWrapper> _databaseCollection;
        private Mock<IDataSourceProcessor> _dataSourceProcessor;
        private Mock<IDataSourceProcessorFactory> _dataSourceProcessorFactory;
        private Mock<ILogger> _logger;
        private Mock<IModelWrapper> _model;
        private Mock<IServerWrapper> _server;
        private Mock<IServerWrapperFactory> _serverFactory;
        private ModelProcessor _sut;
        private Mock<ITableProcessor> _tableProcessor;
        private Mock<ITokenProvider> _tokenProvider;
        private Mock<ITokenProviderFactory> _tokenProviderFactory;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration(),
                DataSource = new DataSourceConfiguration(),
                DatabaseName = "ModelDatabaseName",
                FullTables = new List<FullTableConfiguration>(),
                PartitionedTables = new List<PartitionedTableConfiguration>(),
                ServerName = "ModelConnectionString"
            };

            _model = new Mock<IModelWrapper>();

            _database = new Mock<IDatabaseWrapper>();
            _database.Setup(x => x.Model).Returns(_model.Object);

            _databaseCollection = new Mock<IDatabaseCollectionWrapper>();
            _databaseCollection.Setup(x => x.FindByName(It.IsAny<string>())).Returns(_database.Object);

            _server = new Mock<IServerWrapper>();
            _server.Setup(x => x.Databases).Returns(_databaseCollection.Object);
            _server.Setup(x => x.Name).Returns(_configuration.ServerName);

            _serverFactory = new Mock<IServerWrapperFactory>();
            _serverFactory.Setup(x => x.Create()).Returns(_server.Object);

            _tokenProvider = new Mock<ITokenProvider>();
            _tokenProvider.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Token
                {
                    AccessToken = "AnalysisServicesToken"
                }));

            _tableProcessor = new Mock<ITableProcessor>();

            _dataSourceProcessor = new Mock<IDataSourceProcessor>();

            _dataSourceProcessorFactory = new Mock<IDataSourceProcessorFactory>();
            _dataSourceProcessorFactory.Setup(x => x.Create(It.IsAny<DataSourceType>()))
                .Returns(_dataSourceProcessor.Object);

            _tokenProviderFactory = new Mock<ITokenProviderFactory>();
            _tokenProviderFactory.Setup(x => x.CreateAnalysisServicesTokenProvider(It.IsAny<ModelConfiguration>()))
                .Returns(_tokenProvider.Object);

            _logger = new Mock<ILogger>();

            _sut = new ModelProcessor(_serverFactory.Object,
                _tableProcessor.Object,
                _dataSourceProcessorFactory.Object,
                _tokenProviderFactory.Object,
                _logger.Object);
        }

        [TestMethod]
        public async Task TestCreatesServerFromFactory()
        {
            await _sut.ProcessAsync(_configuration);
            _serverFactory.Verify(x => x.Create());
        }

        [TestMethod]
        public async Task TestConnectsToServerWithModelConnectionString()
        {
            var expected =
                $"Data Source={_configuration.ServerName};Password=AnalysisServicesToken;Provider=MSOLAP;";

            await _sut.ProcessAsync(_configuration);
            _server.Verify(x => x.Connect(expected));
        }

        [TestMethod]
        public async Task TestFindsDatabaseInServer()
        {
            await _sut.ProcessAsync(_configuration);
            _databaseCollection.Verify(x => x.FindByName(_configuration.DatabaseName));
        }

        [TestMethod]
        public async Task TestInvalidDatabaseInServerThrowsConnectionException()
        {
            _databaseCollection.Setup(x => x.FindByName(It.IsAny<string>())).Returns((IDatabaseWrapper) null);
            await Assert.ThrowsExceptionAsync<ConnectionException>(() => _sut.ProcessAsync(_configuration));
        }

        [TestMethod]
        public async Task TestGetsAnalysisServicesTokenUsingModel()
        {
            await _sut.ProcessAsync(_configuration);
            _tokenProviderFactory.Verify(x => x.CreateAnalysisServicesTokenProvider(_configuration));
        }

        [TestMethod]
        public async Task TestProcessesTables()
        {
            await _sut.ProcessAsync(_configuration);
            _tableProcessor.Verify(x => x.Process(_database.Object, _configuration));
        }

        [TestMethod]
        public async Task TestGetsDataSourceProcessorUsingDataSourceType()
        {
            await _sut.ProcessAsync(_configuration);
            _dataSourceProcessorFactory.Verify(x => x.Create(_configuration.DataSource.Type));
        }

        [TestMethod]
        public async Task TestProcessesDataSource()
        {
            await _sut.ProcessAsync(_configuration);
            _dataSourceProcessor.Verify(x => x.ProcessAsync(_database.Object, _configuration));
        }

        [TestMethod]
        public async Task TestCallsDatabaseSaveChangesWithMaxParallelism()
        {
            await _sut.ProcessAsync(_configuration);
            _model.Verify(x => x.SaveChanges(It.Is<SaveOptions>(y => y.MaxParallelism == 5)));
        }

        [TestMethod]
        public async Task TestCallsModelRecalculate()
        {
            await _sut.ProcessAsync(_configuration);
            _model.Verify(x => x.RequestRefresh(RefreshType.Calculate));
        }

        [TestMethod]
        public async Task TestCallsDatabaseSaveChanges()
        {
            await _sut.ProcessAsync(_configuration);
            _model.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public async Task TestLogsStartingProcessing()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Starting processing."));
        }

        [TestMethod]
        public async Task TestLogsGetAnalysisServicesAuthenticationToken()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Retrieved Analysis Services authentication token."));
        }

        [TestMethod]
        public async Task TestLogsConnectToServer()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info($"Connected to server {_configuration.ServerName}."));
        }

        [TestMethod]
        public async Task TestLogsConnectToDatabase()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info($"Connected to database {_configuration.DatabaseName}."));
        }

        [TestMethod]
        public async Task TestLogsSavingModelChanges()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Saving model changes."));
        }

        [TestMethod]
        public async Task TestLogsRecalculatingModel()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Recalculating model."));
        }

        [TestMethod]
        public async Task TestLogsFinishedProcessing()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Finished processing."));
        }

        [TestMethod]
        public async Task TestLogsException()
        {
            _server.Setup(x => x.Connect(It.IsAny<string>())).Throws(new ConnectionException("Test exception."));

            try
            {
                await _sut.ProcessAsync(_configuration);
            }
            catch
            {
                // ignored
            }

            _logger.Verify(x => x.Error(It.IsAny<Exception>()));
        }
    }
}