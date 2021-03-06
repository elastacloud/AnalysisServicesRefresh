﻿using System.Threading;
using System.Threading.Tasks;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AnalysisServicesRefresh.BLL.Tokens
{
    public class Token : IToken
    {
        private readonly string _authority;
        private readonly Models.ClientCredential _credential;
        private readonly string _resource;

        public Token(Models.ClientCredential credential, string authority, string resource)
        {
            _credential = credential;
            _authority = authority;
            _resource = resource;
        }

        public async Task<Models.Token> GetAsync(CancellationToken cancellationToken = default)
        {
            var context = new AuthenticationContext(_authority);
            var token = await context.AcquireTokenAsync(_resource,
                new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(_credential.Username, _credential.Password));

            return new Models.Token
            {
                AccessToken = token.AccessToken,
                ExpiresOn = token.ExpiresOn
            };
        }
    }
}