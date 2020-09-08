using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.KeyVaults;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class CredentialTests
    {
        private AuthenticationConfiguration _configuration;
        private Mock<IKeyVault> _keyVault;
        private Mock<IKeyVaultFactory> _keyVaultFactory;
        private Credential _sut;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new AuthenticationConfiguration
            {
                KeyVaultBaseUri = "KeyVaultBaseUri",
                KeyVaultClientId = "KeyVaultClientId",
                KeyVaultAuthentication = "KeyVaultAuthentication"
            };

            _keyVault = new Mock<IKeyVault>();
            _keyVault.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ActiveDirectoryClientCredential
                {
                    ClientId = "DataSourceClientIdFromKeyVault",
                    ClientSecret = "DataSourceClientSecretFromKeyVault"
                }));

            _keyVaultFactory = new Mock<IKeyVaultFactory>();
            _keyVaultFactory.Setup(x => x.Create(It.IsAny<AuthenticationType>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(_keyVault.Object);

            _sut = new Credential(_keyVaultFactory.Object, _configuration);
        }

        [TestMethod]
        public async Task TestDoesNotRetrieveCredentialsFromKeyVaultWhenKeyVaultBaseUriIsNull()
        {
            _configuration.KeyVaultBaseUri = null;
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVaultFactory.Verify(
                x => x.Create(It.IsAny<AuthenticationType>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task TestRetrievesCredentialsFromKeyVaultWhenKeyVaultBaseUriIsNotNull()
        {
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVaultFactory.Verify(
                x => x.Create(It.IsAny<AuthenticationType>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task TestReturnsCredentialsPassedInWhenKeyVaultBaseUriIsNull()
        {
            _configuration.KeyVaultBaseUri = null;
            var actual = await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");

            Assert.AreEqual("DataSourceClientId", actual.ClientId);
            Assert.AreEqual("DataSourceClientSecret", actual.ClientSecret);
        }

        [TestMethod]
        public async Task TestGetsCredentialsFromKeyVaultUsingAuthenticationType()
        {
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVaultFactory.Verify(x =>
                x.Create(_configuration.Type, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TestGetsCredentialsFromKeyVaultUsingBaseUri()
        {
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVaultFactory.Verify(x => x.Create(It.IsAny<AuthenticationType>(), _configuration.KeyVaultBaseUri,
                It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TestGetsCredentialsFromKeyVaultUsingClientId()
        {
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVaultFactory.Verify(x => x.Create(It.IsAny<AuthenticationType>(), It.IsAny<string>(),
                _configuration.KeyVaultClientId, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TestGetsCredentialsFromKeyVaultUsingAuthentication()
        {
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVaultFactory.Verify(x => x.Create(It.IsAny<AuthenticationType>(), It.IsAny<string>(),
                It.IsAny<string>(), _configuration.KeyVaultAuthentication));
        }

        [TestMethod]
        public async Task TestGetsCredentialsFromKeyVaultUsingDataSourceClientId()
        {
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVault.Verify(x => x.GetAsync("DataSourceClientId", It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestGetsCredentialsFromKeyVaultUsingDataSourceClientSecret()
        {
            await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");
            _keyVault.Verify(x =>
                x.GetAsync(It.IsAny<string>(), "DataSourceClientSecret", It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestReturnsCredentialsFromKeyVaultWhenKeyVaultBaseUriIsNotNull()
        {
            var actual = await _sut.GetAsync("DataSourceClientId", "DataSourceClientSecret");

            Assert.AreEqual("DataSourceClientIdFromKeyVault", actual.ClientId);
            Assert.AreEqual("DataSourceClientSecretFromKeyVault", actual.ClientSecret);
        }
    }
}