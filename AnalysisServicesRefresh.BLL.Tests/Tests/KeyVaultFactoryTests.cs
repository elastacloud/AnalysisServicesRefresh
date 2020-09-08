using System;
using AnalysisServicesRefresh.BLL.KeyVaults;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class KeyVaultFactoryTests
    {
        private KeyVaultFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new KeyVaultFactory();
            ;
        }

        [TestMethod]
        public void TestCreatesCertificateKeyVault()
        {
            var actual = _sut.Create(AuthenticationType.Certificate, "", "", "");
            Assert.IsInstanceOfType(actual, typeof(CertificateKeyVault));
        }

        [TestMethod]
        public void TestCreatesClientSecretKeyVault()
        {
            var actual = _sut.Create(AuthenticationType.Secret, "", "", "");
            Assert.IsInstanceOfType(actual, typeof(ClientSecretKeyVault));
        }

        [TestMethod]
        public void ThrowsInvalidOperationExceptionWhenAuthenticationTypeIsUnknown()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _sut.Create((AuthenticationType) 99, "", "", ""));
        }
    }
}