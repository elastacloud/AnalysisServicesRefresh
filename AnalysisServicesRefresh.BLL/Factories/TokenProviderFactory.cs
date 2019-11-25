using System;
using AnalysisServicesRefresh.BLL.BLL;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.BLL.Factories
{
    public class TokenProviderFactory : ITokenProviderFactory
    {
        public ITokenProvider CreateAnalysisServicesTokenProvider(ModelConfiguration model)
        {
            switch (model.Authentication.Type)
            {
                case AuthenticationType.Certificate:
                    return new AnalysisServicesCertificateTokenProvider(model);
                case AuthenticationType.Secret:
                    return new AnalysisServicesClientSecretTokenProvider(model);
                default: throw new InvalidOperationException("AuthenticationType is not supported.");
            }
        }

        public ITokenProvider CreateSqlServerTokenProvider(ModelConfiguration model)
        {
            switch (model.Authentication.Type)
            {
                case AuthenticationType.Certificate:
                    return new SqlServerOAuthCertificateTokenProvider(model);
                case AuthenticationType.Secret:
                    return new SqlServerOAuthClientSecretTokenProvider(model);
                default: throw new InvalidOperationException("AuthenticationType is not supported.");
            }
        }
    }
}