using System.Collections.Generic;
using System.Linq;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class TableProcessorTests
    {
        private ModelConfiguration _configuration;
        private Mock<IDatabaseWrapper> _database;
        private Mock<IRefresh> _fullRefresh;
        private Mock<ITableWrapper> _fullTable;
        private Mock<ILogger> _logger;
        private Mock<IModelWrapper> _model;
        private Mock<IRefresh> _partitionedRefresh;
        private Mock<ITableWrapper> _partitionedTable;
        private Mock<IRefreshFactory> _refreshFactory;
        private TableProcessor _sut;
        private Mock<ITableCollectionWrapper> _tableCollection;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration(),
                DataSource = new DataSourceConfiguration(),
                DatabaseName = "ModelDatabaseName",
                FullTables = new List<FullTableConfiguration>
                {
                    new FullTableConfiguration
                    {
                        Name = "FullModelTableName"
                    }
                },
                PartitionedTables = new List<PartitionedTableConfiguration>
                {
                    new PartitionedTableConfiguration
                    {
                        Name = "PartitionedModelTableName",
                        Partitions = new List<PartitionConfiguration>()
                    }
                },
                ServerName = "ModelConnectionString"
            };

            _fullRefresh = new Mock<IRefresh>();

            _partitionedRefresh = new Mock<IRefresh>();

            _refreshFactory = new Mock<IRefreshFactory>();
            _refreshFactory.Setup(x => x.CreateFull()).Returns(_fullRefresh.Object);
            _refreshFactory.Setup(x => x.CreatePartitioned(It.IsAny<PartitionedTableConfiguration>()))
                .Returns(_partitionedRefresh.Object);

            _logger = new Mock<ILogger>();

            _fullTable = new Mock<ITableWrapper>();
            _partitionedTable = new Mock<ITableWrapper>();

            _tableCollection = new Mock<ITableCollectionWrapper>();
            _tableCollection.Setup(x => x.Find("FullModelTableName")).Returns(_fullTable.Object);
            _tableCollection.Setup(x => x.Find("PartitionedModelTableName")).Returns(_partitionedTable.Object);

            _model = new Mock<IModelWrapper>();
            _model.Setup(x => x.Tables).Returns(_tableCollection.Object);

            _database = new Mock<IDatabaseWrapper>();
            _database.Setup(x => x.Model).Returns(_model.Object);

            _sut = new TableProcessor(_refreshFactory.Object, _logger.Object);
        }

        [TestMethod]
        public void TestFindsFullTableInDatabase()
        {
            _sut.Process(_database.Object, _configuration);
            _tableCollection.Verify(x => x.Find(_configuration.FullTables.First().Name));
        }

        [TestMethod]
        public void TestFindsPartitionedTableInDatabase()
        {
            _sut.Process(_database.Object, _configuration);
            _tableCollection.Verify(x => x.Find(_configuration.PartitionedTables.First().Name));
        }

        [TestMethod]
        public void TestInvalidTableInDatabaseThrowsConnectionException()
        {
            _tableCollection.Setup(x => x.Find(It.IsAny<string>())).Returns((ITableWrapper) null);
            Assert.ThrowsException<ConnectionException>(() => _sut.Process(_database.Object, _configuration));
        }

        [TestMethod]
        public void TestGetsFullRefreshImplementationFromFactory()
        {
            _sut.Process(_database.Object, _configuration);
            _refreshFactory.Verify(x => x.CreateFull());
        }

        [TestMethod]
        public void TestGetsPartitionedRefreshImplementationFromFactory()
        {
            _sut.Process(_database.Object, _configuration);
            _refreshFactory.Verify(x => x.CreatePartitioned(_configuration.PartitionedTables.First()));
        }

        [TestMethod]
        public void TestCallsRefreshOnFullImplementation()
        {
            _sut.Process(_database.Object, _configuration);
            _fullRefresh.Verify(x => x.Refresh(_fullTable.Object));
        }

        [TestMethod]
        public void TestCallsRefreshOnPartitionedImplementation()
        {
            _sut.Process(_database.Object, _configuration);
            _partitionedRefresh.Verify(x => x.Refresh(_partitionedTable.Object));
        }

        [TestMethod]
        public void TestLogsFetchTable()
        {
            _sut.Process(_database.Object, _configuration);
            _logger.Verify(x => x.Info($"Fetched table {_configuration.FullTables.First().Name}."));
            _logger.Verify(x => x.Info($"Fetched table {_configuration.PartitionedTables.First().Name}."));
        }

        [TestMethod]
        public void TestLogsProcessingTable()
        {
            _sut.Process(_database.Object, _configuration);
            _logger.Verify(x => x.Info($"Processing table {_configuration.FullTables.First().Name}."));
            _logger.Verify(x => x.Info($"Processing table {_configuration.PartitionedTables.First().Name}."));
        }
    }
}