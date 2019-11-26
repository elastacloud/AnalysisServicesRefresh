using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class ServerWrapperFactoryTests
    {
        [TestMethod]
        public void TestCreatesServerWrapper()
        {
            var sut = new ServerWrapperFactory();
            var actual = sut.Create();
            Assert.IsInstanceOfType(actual, typeof(ServerWrapper));
        }
    }
}