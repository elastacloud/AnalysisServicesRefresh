using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class CredentialFactoryTests
    {
        [TestMethod]
        public void TestCreatesCredential()
        {
            var sut = new CredentialFactory();
            var actual = sut.Create(new AuthenticationConfiguration());
            Assert.IsInstanceOfType(actual, typeof(Credential));
        }
    }
}