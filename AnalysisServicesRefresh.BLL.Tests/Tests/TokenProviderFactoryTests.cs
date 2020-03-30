using System;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class TokenProviderFactoryTests
    {
        private TokenProviderFactory _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new TokenProviderFactory();
        }

        [TestMethod]
        public void TestCreatesAnalysisServicesCertificateTokenProvider()
        {
            var actual = _sut.CreateAnalysisServicesTokenProvider(new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    Type = AuthenticationType.Certificate
                }
            });
            Assert.IsInstanceOfType(actual, typeof(AnalysisServicesCertificateTokenProvider));
        }

        [TestMethod]
        public void TestCreatesAnalysisServicesClientSecretTokenProvider()
        {
            var actual = _sut.CreateAnalysisServicesTokenProvider(new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    Type = AuthenticationType.Secret
                }
            });
            Assert.IsInstanceOfType(actual, typeof(AnalysisServicesClientSecretTokenProvider));
        }

        [TestMethod]
        public void ThrowsInvalidOperationExceptionWhenAuthenticationTypeUnknownForAnalysisServicesTokenProvider()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _sut.CreateAnalysisServicesTokenProvider(new ModelConfiguration
                {
                    Authentication = new AuthenticationConfiguration
                    {
                        Type = (AuthenticationType) 99
                    }
                }));
        }

        [TestMethod]
        public void TestCreatesSqlServerCertificateTokenProvider()
        {
            var actual = _sut.CreateSqlServerTokenProvider(new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    Type = AuthenticationType.Certificate
                }
            });
            Assert.IsInstanceOfType(actual, typeof(SqlServerOAuthCertificateTokenProvider));
        }

        [TestMethod]
        public void TestCreatesSqlServerClientSecretTokenProvider()
        {
            var actual = _sut.CreateSqlServerTokenProvider(new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    Type = AuthenticationType.Secret
                }
            });
            Assert.IsInstanceOfType(actual, typeof(SqlServerOAuthClientSecretTokenProvider));
        }

        [TestMethod]
        public void ThrowsInvalidOperationExceptionWhenAuthenticationTypeUnknownForSqlServerTokenProvider()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _sut.CreateSqlServerTokenProvider(new ModelConfiguration
                {
                    Authentication = new AuthenticationConfiguration
                    {
                        Type = (AuthenticationType) 99
                    }
                }));
        }
    }
}