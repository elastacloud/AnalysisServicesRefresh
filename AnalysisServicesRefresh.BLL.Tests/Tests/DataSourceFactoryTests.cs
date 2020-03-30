using System;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class DataSourceFactoryTests
    {
        private DataSourceFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new DataSourceFactory();
        }

        [TestMethod]
        public void TestCreatesOAuthDataSource()
        {
            var actual = _sut.Create(DataSourceType.OAuth);
            Assert.IsInstanceOfType(actual, typeof(SqlServerOAuthDataSource));
        }

        [TestMethod]
        public void TestCreatesPassthroughDataSource()
        {
            var actual = _sut.Create(DataSourceType.Passthrough);
            Assert.IsInstanceOfType(actual, typeof(PassthroughDataSource));
        }

        [TestMethod]
        public void TestThrowsInvalidOperationExceptionWhenDataSourceTypeIsUnknown()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.Create((DataSourceType) 99));
        }
    }
}