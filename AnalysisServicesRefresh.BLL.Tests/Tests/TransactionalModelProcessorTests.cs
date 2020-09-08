using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.ConnectionStrings;
using AnalysisServicesRefresh.BLL.DataSources;
using AnalysisServicesRefresh.BLL.ModelProcessors;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Refreshes;
using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using DataSourceType = AnalysisServicesRefresh.BLL.DataSources.DataSourceType;
using RefreshType = Microsoft.AnalysisServices.Tabular.RefreshType;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class TransactionalModelProcessorTests
    {
        private ModelConfiguration _configuration;
        private Mock<IConnectionString> _connectionString;
        private Mock<IConnectionStringFactory> _connectionStringFactory;
        private Mock<IDatabaseWrapper> _database;
        private Mock<IDatabaseCollectionWrapper> _databaseCollection;
        private Mock<IDataSource> _dataSource;
        private Mock<IDataSourceFactory> _dataSourceFactory;
        private Mock<IRefresh> _fullRefresh;
        private Mock<ITableWrapper> _fullTable;
        private Mock<ILogger> _logger;
        private Mock<IModelWrapper> _model;
        private Mock<IRefresh> _partitionedRefresh;
        private Mock<ITableWrapper> _partitionedTable;
        private Mock<IRefreshFactory> _refreshFactory;
        private Mock<IServerWrapper> _server;
        private Mock<IServerWrapperFactory> _serverFactory;
        private TransactionalModelProcessor _sut;
        private Mock<ITableCollectionWrapper> _tableCollection;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration(),
                DataSource = new DataSourceConfiguration
                {
                    Type = DataSourceType.OAuth
                },
                DatabaseName = "DatabaseName",
                FullTables = new List<FullTableConfiguration>
                {
                    new FullTableConfiguration
                    {
                        Name = "FullTableName"
                    }
                },
                PartitionedTables = new List<PartitionedTableConfiguration>
                {
                    new PartitionedTableConfiguration
                    {
                        Name = "PartitionedTableName",
                        Partitions = new List<PartitionConfiguration>()
                    }
                },
                ServerName = "ServerName"
            };

            _fullTable = new Mock<ITableWrapper>();
            _fullTable.Setup(x => x.Name).Returns("FullTableName");

            _partitionedTable = new Mock<ITableWrapper>();
            _partitionedTable.Setup(x => x.Name).Returns("PartitionedTableName");

            _tableCollection = new Mock<ITableCollectionWrapper>();
            _tableCollection.Setup(x => x.Find("FullTableName")).Returns(_fullTable.Object);
            _tableCollection.Setup(x => x.Find("PartitionedTableName")).Returns(_partitionedTable.Object);

            _model = new Mock<IModelWrapper>();
            _model.Setup(x => x.Tables).Returns(_tableCollection.Object);

            _database = new Mock<IDatabaseWrapper>();
            _database.Setup(x => x.Model).Returns(_model.Object);

            _databaseCollection = new Mock<IDatabaseCollectionWrapper>();
            _databaseCollection.Setup(x => x.FindByName(It.IsAny<string>())).Returns(_database.Object);

            _server = new Mock<IServerWrapper>();
            _server.Setup(x => x.Databases).Returns(_databaseCollection.Object);
            _server.Setup(x => x.Name).Returns(_configuration.ServerName);

            _serverFactory = new Mock<IServerWrapperFactory>();
            _serverFactory.Setup(x => x.Create()).Returns(_server.Object);

            _connectionString = new Mock<IConnectionString>();
            _connectionString.Setup(x => x.GetAsync(It.IsAny<ModelConfiguration>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult("ConnectionString"));

            _connectionStringFactory = new Mock<IConnectionStringFactory>();
            _connectionStringFactory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns(_connectionString.Object);

            _fullRefresh = new Mock<IRefresh>();

            _partitionedRefresh = new Mock<IRefresh>();

            _refreshFactory = new Mock<IRefreshFactory>();
            _refreshFactory.Setup(x => x.CreateFull()).Returns(_fullRefresh.Object);
            _refreshFactory.Setup(x => x.CreatePartitioned(It.IsAny<PartitionedTableConfiguration>()))
                .Returns(_partitionedRefresh.Object);

            _dataSource = new Mock<IDataSource>();

            _dataSourceFactory = new Mock<IDataSourceFactory>();
            _dataSourceFactory.Setup(x => x.Create(It.IsAny<DataSourceType>()))
                .Returns(_dataSource.Object);

            _logger = new Mock<ILogger>();

            _sut = new TransactionalModelProcessor(
                _serverFactory.Object,
                _connectionStringFactory.Object,
                _refreshFactory.Object,
                _dataSourceFactory.Object,
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
            await _sut.ProcessAsync(_configuration);
            _server.Verify(x => x.Connect("ConnectionString"));
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
        public async Task TestGetsConnectionStringUsingServerName()
        {
            await _sut.ProcessAsync(_configuration);
            _connectionStringFactory.Verify(x => x.Create(_configuration.ServerName));
        }

        [TestMethod]
        public async Task TestGetsConnectionStringUsingModelConfiguration()
        {
            await _sut.ProcessAsync(_configuration);
            _connectionString.Verify(x => x.GetAsync(_configuration, It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestLogsStartingProcessing()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Starting processing."));
        }

        [TestMethod]
        public async Task TestLogsGetConnectionString()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Retrieved connection string."));
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

        [TestMethod]
        public async Task TestFindsFullTableInDatabase()
        {
            await _sut.ProcessAsync(_configuration);
            _tableCollection.Verify(x => x.Find(_configuration.FullTables.First().Name));
        }

        [TestMethod]
        public async Task TestFindsPartitionedTableInDatabase()
        {
            await _sut.ProcessAsync(_configuration);
            _tableCollection.Verify(x => x.Find(_configuration.PartitionedTables.First().Name));
        }

        [TestMethod]
        public async Task TestInvalidTableInDatabaseThrowsConnectionException()
        {
            _tableCollection.Setup(x => x.Find(It.IsAny<string>())).Returns((ITableWrapper) null);
            await Assert.ThrowsExceptionAsync<ConnectionException>(() => _sut.ProcessAsync(_configuration));
        }

        [TestMethod]
        public async Task TestGetsFullRefreshImplementationFromFactory()
        {
            await _sut.ProcessAsync(_configuration);
            _refreshFactory.Verify(x => x.CreateFull());
        }

        [TestMethod]
        public async Task TestGetsPartitionedRefreshImplementationFromFactory()
        {
            await _sut.ProcessAsync(_configuration);
            _refreshFactory.Verify(x => x.CreatePartitioned(_configuration.PartitionedTables.First()));
        }

        [TestMethod]
        public async Task TestCallsRefreshOnFullImplementation()
        {
            await _sut.ProcessAsync(_configuration);
            _fullRefresh.Verify(x => x.Refresh(_fullTable.Object));
        }

        [TestMethod]
        public async Task TestCallsRefreshOnPartitionedImplementation()
        {
            await _sut.ProcessAsync(_configuration);
            _partitionedRefresh.Verify(x => x.Refresh(_partitionedTable.Object));
        }

        [TestMethod]
        public async Task TestGetsDataSourceProcessorUsingDataSourceType()
        {
            await _sut.ProcessAsync(_configuration);
            _dataSourceFactory.Verify(x => x.Create(_configuration.DataSource.Type));
        }

        [TestMethod]
        public async Task TestProcessesDataSource()
        {
            await _sut.ProcessAsync(_configuration);
            _dataSource.Verify(x => x.ProcessAsync(_database.Object, _configuration, It.IsAny<CancellationToken>()),
                Times.Once);
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
        public async Task TestDisconnectsFromServer()
        {
            await _sut.ProcessAsync(_configuration);
            _server.Verify(x => x.Disconnect());
        }

        [TestMethod]
        public async Task TestDisposesServer()
        {
            await _sut.ProcessAsync(_configuration);
            _server.Verify(x => x.Dispose());
        }

        [TestMethod]
        public async Task TestDoesNotContinueProcessingIfTableProcessingThrowsException()
        {
            _model.Setup(x => x.SaveChanges(It.IsAny<SaveOptions>()))
                .Throws<Exception>();

            try
            {
                await _sut.ProcessAsync(_configuration);
            }
            catch
            {
                // ignored
            }

            _model.Verify(x => x.SaveChanges(It.IsAny<SaveOptions>()), Times.Once);
        }

        [TestMethod]
        public async Task TestLogsFetchTable()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info($"Fetched table {_configuration.FullTables.First().Name}."));
            _logger.Verify(x => x.Info($"Fetched table {_configuration.PartitionedTables.First().Name}."));
        }

        [TestMethod]
        public async Task TestLogsProcessingTable()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info($"Processing table {_configuration.FullTables.First().Name}."));
            _logger.Verify(x => x.Info($"Processing table {_configuration.PartitionedTables.First().Name}."));
        }

        [TestMethod]
        public async Task TestLogsSavingModelChanges()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Saving model changes."), Times.Once);
        }

        [TestMethod]
        public async Task TestLogsRecalculatingModel()
        {
            await _sut.ProcessAsync(_configuration);
            _logger.Verify(x => x.Info("Recalculating model."), Times.Once);
        }
    }
}