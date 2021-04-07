using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.ConnectionStrings;
using AnalysisServicesRefresh.BLL.Credentials;
using AnalysisServicesRefresh.BLL.Models;
using AnalysisServicesRefresh.BLL.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Token = AnalysisServicesRefresh.BLL.Models.Token;

namespace AnalysisServicesRefresh.BLL.Tests.Tests
{
    [TestClass]
    public class AnalysisServicesConnectionStringTests
    {
        private ModelConfiguration _configuration;
        private Mock<ICredential> _credential;
        private Mock<ICredentialFactory> _credentialFactory;
        private AnalysisServicesConnectionString _sut;
        private Mock<IToken> _token;
        private Mock<ITokenFactory> _tokenFactory;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ModelConfiguration
            {
                Authentication = new AuthenticationConfiguration
                {
                    DirectoryId = "DirectoryId",
                    AnalysisServicesClientId = "AnalysisServicesClientId",
                    AnalysisServicesClientSecret = "AnalysisServicesClientSecret"
                },
                DataSource = new DataSourceConfiguration(),
                DatabaseName = "DatabaseName",
                FullTables = new List<FullTableConfiguration>(),
                PartitionedTables = new List<PartitionedTableConfiguration>(),
                ServerName = "ServerName"
            };

            _credential = new Mock<ICredential>();
            _credential.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ClientCredential
                {
                    Username = "AnalysisServicesClientIdFromCredential",
                    Password = "AnalysisServicesClientSecretFromCredential"
                }));

            _credentialFactory = new Mock<ICredentialFactory>();
            _credentialFactory.Setup(x => x.Create(It.IsAny<AuthenticationConfiguration>()))
                .Returns(_credential.Object);

            _token = new Mock<IToken>();
            _token.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Token
                {
                    AccessToken = "AnalysisServicesToken",
                    ExpiresOn = new DateTimeOffset(2019, 10, 24, 12, 0, 0, TimeSpan.Zero)
                }));

            _tokenFactory = new Mock<ITokenFactory>();
            _tokenFactory.Setup(x =>
                    x.Create(It.IsAny<ClientCredential>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_token.Object);

            _sut = new AnalysisServicesConnectionString(_credentialFactory.Object, _tokenFactory.Object);
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingModelAuthentication()
        {
            await _sut.GetAsync(_configuration);
            _credentialFactory.Verify(x => x.Create(_configuration.Authentication));
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingAnalysisServicesClientId()
        {
            await _sut.GetAsync(_configuration);
            _credential.Verify(x =>
                x.GetAsync("AnalysisServicesClientId", It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestGetsGetsCredentialUsingAnalysisServicesClientSecret()
        {
            await _sut.GetAsync(_configuration);
            _credential.Verify(x =>
                x.GetAsync(It.IsAny<string>(), "AnalysisServicesClientSecret", It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task TestGetsAnalysisServicesTokenUsingAnalysisServicesClientIdFromCredential()
        {
            await _sut.GetAsync(_configuration);
            _tokenFactory.Verify(x =>
                x.Create(
                    It.Is<ClientCredential>(y => y.Username == "AnalysisServicesClientIdFromCredential"),
                    It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TestGetsAnalysisServicesTokenUsingAnalysisServicesClientSecretFromCredential()
        {
            await _sut.GetAsync(_configuration);
            _tokenFactory.Verify(x =>
                x.Create(
                    It.Is<ClientCredential>(y =>
                        y.Password == "AnalysisServicesClientSecretFromCredential"), It.IsAny<string>(),
                    It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TestGetsAnalysisServicesTokenUsingAuthority()
        {
            await _sut.GetAsync(_configuration);
            _tokenFactory.Verify(x => x.Create(It.IsAny<ClientCredential>(),
                $"https://login.microsoftonline.com/{_configuration.Authentication.DirectoryId}", It.IsAny<string>()));
        }

        [TestMethod]
        public async Task TestGetsAnalysisServicesTokenUsingResource()
        {
            await _sut.GetAsync(_configuration);
            _tokenFactory.Verify(x => x.Create(It.IsAny<ClientCredential>(), It.IsAny<string>(),
                "https://*.asazure.windows.net/"));
        }

        [TestMethod]
        public async Task TestReturnsAnalysisServicesConnectionString()
        {
            var actual = await _sut.GetAsync(_configuration);

            Assert.AreEqual("Data Source=ServerName;Password=AnalysisServicesToken;Provider=MSOLAP;", actual);
        }
    }
}