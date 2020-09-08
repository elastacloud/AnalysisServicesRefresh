﻿using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Tokens
{
    public class TokenFactory : ITokenFactory
    {
        public IToken Create(ActiveDirectoryClientCredential credential, string authority, string resource)
        {
            return new Token(credential, authority, resource);
        }
    }
}