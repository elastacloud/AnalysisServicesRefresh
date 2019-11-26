using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Interfaces;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class FullRefreshTests
    {
        private Mock<ILogger> _logger;
        private FullRefresh _sut;
        private Mock<ITableWrapper> _table;

        [TestInitialize]
        public void Setup()
        {
            _table = new Mock<ITableWrapper>();
            _table.Setup(x => x.RequestRefresh(It.IsAny<RefreshType>()));
            _table.Setup(x => x.Name).Returns("TableName");

            _logger = new Mock<ILogger>();

            _sut = new FullRefresh(_logger.Object);
        }

        [TestMethod]
        public void TestFullRefreshesTable()
        {
            _sut.Refresh(_table.Object);
            _table.Verify(x => x.RequestRefresh(RefreshType.Full));
        }

        [TestMethod]
        public void TestLogsFullRefresh()
        {
            _sut.Refresh(_table.Object);
            _logger.Verify(x => x.Info($"Requesting full refresh for table {_table.Object.Name}."));
        }
    }
}