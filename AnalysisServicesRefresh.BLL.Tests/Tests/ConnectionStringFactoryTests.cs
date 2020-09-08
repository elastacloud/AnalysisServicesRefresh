using System;
using AnalysisServicesRefresh.BLL.ConnectionStrings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class ConnectionStringFactoryTests
    {
        private ConnectionStringFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new ConnectionStringFactory();
        }

        [TestMethod]
        public void TestCreatesAnalysisServicesConnectionString()
        {
            var actual = _sut.Create("asazure://");
            Assert.IsInstanceOfType(actual, typeof(AnalysisServicesConnectionString));
        }

        [TestMethod]
        public void TestCreatesPowerBiConnectionString()
        {
            var actual = _sut.Create("powerbi://");
            Assert.IsInstanceOfType(actual, typeof(PowerBiConnectionString));
        }

        [TestMethod]
        public void TestThrowsInvalidOperationExceptionWhenConnectionStringTypeIsUnknown()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.Create("unknown://server.com"));
        }
    }
}