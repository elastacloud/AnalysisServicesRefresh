using System;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class DataSourceProcessorFactoryTests
    {
        private DataSourceProcessorFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new DataSourceProcessorFactory();
        }

        [TestMethod]
        public void TestCreatesOAuthDataSourceProcessor()
        {
            var actual = _sut.Create(DataSourceType.OAuth);
            Assert.IsInstanceOfType(actual, typeof(SqlServerOAuthDataSourceProcessor));
        }

        [TestMethod]
        public void TestCreatesPassthroughDataSourceProcessor()
        {
            var actual = _sut.Create(DataSourceType.Passthrough);
            Assert.IsInstanceOfType(actual, typeof(PassthroughDataSourceProcessor));
        }

        [TestMethod]
        public void TestThrowsInvalidOperationExceptionWhenDataSourceTypeIsUnknown()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.Create((DataSourceType) 99));
        }
    }
}