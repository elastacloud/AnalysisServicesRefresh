using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class PassthroughDataSourceProcessorTests
    {
        [TestMethod]
        public void TestProcessReturnsACompletedTask()
        {
            var database = new Mock<IDatabaseWrapper>();
            var configuration = new ModelConfiguration();
            var sut = new PassthroughDataSourceProcessor();
            var actual = sut.ProcessAsync(database.Object, configuration);
            Assert.AreEqual(Task.CompletedTask, actual);
        }
    }
}