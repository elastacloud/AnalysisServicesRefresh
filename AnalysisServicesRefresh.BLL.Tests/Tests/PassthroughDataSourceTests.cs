using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.DataSources;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class PassthroughDataSourceTests
    {
        [TestMethod]
        public void TestProcessReturnsACompletedTask()
        {
            var database = new Mock<IDatabaseWrapper>();
            var configuration = new ModelConfiguration();
            var sut = new PassthroughDataSource();
            var actual = sut.ProcessAsync(database.Object, configuration);
            Assert.AreEqual(Task.CompletedTask, actual);
        }
    }
}