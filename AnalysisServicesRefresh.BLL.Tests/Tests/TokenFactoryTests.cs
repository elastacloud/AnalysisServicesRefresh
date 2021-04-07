using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Token = AnalysisServicesRefresh.BLL.Tokens.Token;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class TokenFactoryTests
    {
        private TokenFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new TokenFactory();
        }

        [TestMethod]
        public void TestCreatesToken()
        {
            var actual = _sut.Create(It.IsAny<ClientCredential>(), It.IsAny<string>(),
                It.IsAny<string>());
            Assert.IsInstanceOfType(actual, typeof(Token));
        }
    }
}