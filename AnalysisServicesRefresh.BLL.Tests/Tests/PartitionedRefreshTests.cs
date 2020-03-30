using System;
using System.Collections.Generic;
using System.Linq;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Tests.Fakes;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using RefreshType = Microsoft.AnalysisServices.Tabular.RefreshType;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class PartitionedRefreshTests
    {
        private PartitionedTableConfiguration _configuration;
        private Mock<ILogger> _logger;
        private Mock<IPartitionCollectionWrapper> _partitionCollection;
        private Mock<IPartitionWrapperFactory> _partitionFactory;
        private PartitionedRefresh _sut;
        private Mock<ITableWrapper> _table;
        private FakePartitionWrapper _templatePartition;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new PartitionedTableConfiguration
            {
                Partitions = new List<PartitionConfiguration>
                {
                    new PartitionConfiguration
                    {
                        Maximum = 20190930,
                        Minimum = 20190901,
                        Name = "20190901",
                        Refresh = false
                    },
                    new PartitionConfiguration
                    {
                        Maximum = 20191031,
                        Minimum = 20191001,
                        Name = "20191001",
                        Refresh = false
                    },
                    new PartitionConfiguration
                    {
                        Maximum = 20191130,
                        Minimum = 20191101,
                        Name = "20191101",
                        Refresh = true
                    },
                    new PartitionConfiguration
                    {
                        Maximum = 20191231,
                        Minimum = 20191201,
                        Name = "20191201",
                        Refresh = false
                    }
                }
            };

            _templatePartition = new FakePartitionWrapper
            {
                Name = "Template",
                Source = new MPartitionSource
                {
                    Expression = "M Expression Where DateKey >= -1 AND DateKey <= -2"
                }
            };

            var collection = new List<IPartitionWrapper>
            {
                _templatePartition,
                new FakePartitionWrapper
                {
                    Name = "DevPartition",
                    Source = new MPartitionSource
                        {Expression = "M Expression Where DateKey >= 20190101 AND DateKey <= 20190201"}
                },
                new FakePartitionWrapper
                {
                    Name = "20190801",
                    Source = new MPartitionSource
                        {Expression = "M Expression Where DateKey >= 20190801 AND DateKey <= 20190831"}
                },
                new FakePartitionWrapper
                {
                    Name = "20190901",
                    Source = new MPartitionSource
                        {Expression = "M Expression Where DateKey >= 20190901 AND DateKey <= 20190930"}
                },
                new FakePartitionWrapper
                {
                    Name = "20191001",
                    Source = new MPartitionSource
                        {Expression = "M Expression Where DateKey >= 20191001 AND DateKey <= 20191031"}
                },
                new FakePartitionWrapper
                {
                    Name = "20191101",
                    Source = new MPartitionSource
                        {Expression = "M Expression Where DateKey >= 20191101 AND DateKey <= 20191130"}
                }
            };

            _partitionFactory = new Mock<IPartitionWrapperFactory>();
            _partitionFactory.Setup(x => x.Create()).Returns(new FakePartitionWrapper());

            _logger = new Mock<ILogger>();

            _partitionCollection = new Mock<IPartitionCollectionWrapper>();
            _partitionCollection.Setup(x => x.Find(It.IsAny<string>())).Returns(_templatePartition);
            _partitionCollection.Setup(x => x.GetEnumerator()).Returns(() => collection.GetEnumerator());

            _partitionCollection.Setup(x => x.Add(It.IsAny<IPartitionWrapper>()))
                .Callback<IPartitionWrapper>(x => collection.Add(x));

            _partitionCollection.Setup(x => x.Remove(It.IsAny<IPartitionWrapper>()))
                .Callback<IPartitionWrapper>(x => collection.Remove(x))
                .Returns(true);

            _table = new Mock<ITableWrapper>();
            _table.Setup(x => x.Partitions).Returns(_partitionCollection.Object);
            _table.Setup(x => x.Name).Returns("PartitionedTable");

            _sut = new PartitionedRefresh(_configuration, _partitionFactory.Object, _logger.Object);
        }

        [TestMethod]
        public void TestRemovesPartitionsInModelAndNotInConfiguration()
        {
            _sut.Refresh(_table.Object);
            Assert.IsFalse(_partitionCollection.Object.Any(x => x.Name == "20190801"));
        }

        [TestMethod]
        public void TestDoesNotRemoveTemplatePartition()
        {
            _sut.Refresh(_table.Object);
            Assert.IsTrue(_partitionCollection.Object.Any(x => x.Name == "Template"));
        }

        [TestMethod]
        public void TestDoesNotRemoveDevPartition()
        {
            _sut.Refresh(_table.Object);
            Assert.IsTrue(_partitionCollection.Object.Any(x => x.Name == "DevPartition"));
        }


        [TestMethod]
        public void TestAddsPartitionsNotInModelAndInConfiguration()
        {
            _sut.Refresh(_table.Object);
            Assert.IsTrue(_partitionCollection.Object.Any(x => x.Name == "20191201"));
        }

        [TestMethod]
        public void TestAddsQueryToExpressionWhenTemplateIsMPartitionSource()
        {
            _sut.Refresh(_table.Object);

            var source =
                (MPartitionSource) _partitionCollection.Object.First(x => x.Name == "20191201").Source;

            Assert.AreEqual("M Expression Where DateKey >= 20191201 AND DateKey <= 20191231",
                source.Expression);
        }

        [TestMethod]
        public void TestRequestsRefreshOnPartitionSpecifiedByQuery()
        {
            _sut.Refresh(_table.Object);

            var partition =
                (FakePartitionWrapper) _partitionCollection.Object.First(x => x.Name == "20191101");

            Assert.AreEqual(RefreshType.Full, partition.RefreshType);
        }

        [TestMethod]
        public void TestThrowArgumentExceptionWhenQueryChangesInOldPartition()
        {
            var update = _configuration.Partitions.First(x => x.Name == "20190901");
            update.Maximum = 20190929;

            Assert.ThrowsException<ArgumentException>(() => _sut.Refresh(_table.Object));
        }

        [TestMethod]
        public void TestThrowArgumentExceptionWhenMinimumOverlapBoundsOfPreviousPartition()
        {
            var update = _configuration.Partitions.First(x => x.Name == "20191201");
            update.Minimum = 20191130;

            Assert.ThrowsException<ArgumentException>(() => _sut.Refresh(_table.Object));
        }

        [TestMethod]
        public void TestThrowArgumentExceptionWhenMaximumOverlapBoundsOfNextPartition()
        {
            _configuration.Partitions.Add(new PartitionConfiguration
            {
                Maximum = 20190901,
                Minimum = 20190801,
                Name = "20190801",
                Refresh = true
            });

            Assert.ThrowsException<ArgumentException>(() => _sut.Refresh(_table.Object));
        }

        [TestMethod]
        public void TestThrowInvalidOperationExceptionWhenTemplatePartitionSourceIsUnsupported()
        {
            _templatePartition.Source = new QueryPartitionSource
            {
                Query = "SELECT * FROM MyTable WHERE DateKey >= -1 AND DateKey <= -2"
            };

            Assert.ThrowsException<InvalidOperationException>(() => _sut.Refresh(_table.Object));
        }

        [TestMethod]
        public void TestThrowConnectionExceptionWhenTemplatePartitionSourceIsNotFound()
        {
            _partitionCollection.Setup(x => x.Find(It.IsAny<string>())).Returns((IPartitionWrapper) null);

            Assert.ThrowsException<ConnectionException>(() => _sut.Refresh(_table.Object));
        }

        [TestMethod]
        public void TestLogsPartitionedRefreshWithInitialLoadInformation()
        {
            _sut.Refresh(_table.Object);
            _logger.Verify(x =>
                x.Info($"Requesting partitioned refresh for table {_table.Object.Name}."));
        }

        [TestMethod]
        public void TestLogsRemovingPartitions()
        {
            _sut.Refresh(_table.Object);
            _logger.Verify(x => x.Info("Removing partitions: 20190801."));
        }

        [TestMethod]
        public void TestLogsAddingPartitions()
        {
            _sut.Refresh(_table.Object);
            _logger.Verify(x => x.Info("Adding partitions: 20191201."));
        }

        [TestMethod]
        public void TestLogsRefreshingPartitions()
        {
            _sut.Refresh(_table.Object);
            _logger.Verify(x => x.Info("Refreshing partitions: 20191101."));
        }

        [TestMethod]
        public void TestRefreshesAllNewPartitionsIfTableIsEmpty()
        {
            _partitionCollection.Object
                .Where(x => x.Name != "Template" && x.Name != "DevPartition")
                .ToList()
                .ForEach(x => _partitionCollection.Object.Remove(x));

            _sut.Refresh(_table.Object);

            Assert.IsTrue(_partitionCollection.Object
                .Where(x => x.Name != "Template" && x.Name != "DevPartition")
                .All(x => ((FakePartitionWrapper) x).RefreshType == RefreshType.Full)
            );
        }
    }
}