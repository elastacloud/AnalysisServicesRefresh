using System;
using AnalysisServicesRefresh.BLL.ModelProcessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class ModelProcessorFactoryTests
    {
        private ModelProcessorFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new ModelProcessorFactory();
        }

        [TestMethod]
        public void TestCreatesNonTransactionalDataSource()
        {
            var actual = _sut.Create(ModelProcessorType.NonTransactional);
            Assert.IsInstanceOfType(actual, typeof(NonTransactionalModelProcessor));
        }

        [TestMethod]
        public void TestCreatesTransactionalDataSource()
        {
            var actual = _sut.Create(ModelProcessorType.Transactional);
            Assert.IsInstanceOfType(actual, typeof(TransactionalModelProcessor));
        }

        [TestMethod]
        public void TestThrowsInvalidOperationExceptionWhenModelProcessorTypeIsUnknown()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.Create((ModelProcessorType) 99));
        }
    }
}