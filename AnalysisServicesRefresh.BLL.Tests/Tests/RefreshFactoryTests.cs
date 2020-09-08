using System.Collections.Generic;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Refreshes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class RefreshFactoryTests
    {
        private PartitionedTableConfiguration _configuration;
        private RefreshFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new PartitionedTableConfiguration
            {
                Partitions = new List<PartitionConfiguration>
                {
                    new PartitionConfiguration
                        {Maximum = 20190930, Minimum = 20190901, Name = "20190901", Refresh = true}
                }
            };

            _sut = new RefreshFactory();
        }

        [TestMethod]
        public void TestCreatesFullRefresh()
        {
            var refresh = _sut.CreateFull();
            Assert.IsInstanceOfType(refresh, typeof(FullRefresh));
        }

        [TestMethod]
        public void TestCreatesDynamicRefresh()
        {
            var refresh = _sut.CreatePartitioned(_configuration);
            Assert.IsInstanceOfType(refresh, typeof(PartitionedRefresh));
        }

        [TestMethod]
        public void TestModelConfigurationIsForwardedToDynamicRefresh()
        {
            var refresh = (PartitionedRefresh) _sut.CreatePartitioned(_configuration);
            Assert.AreEqual(_configuration, refresh.PartitionedTable);
        }
    }
}