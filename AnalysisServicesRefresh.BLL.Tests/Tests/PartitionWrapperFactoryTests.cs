using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class PartitionWrapperFactoryTests
    {
        [TestMethod]
        public void TestCreatesPartitionWrapper()
        {
            var sut = new PartitionWrapperFactory();
            var actual = sut.Create();
            Assert.IsInstanceOfType(actual, typeof(PartitionWrapper));
        }
    }
}