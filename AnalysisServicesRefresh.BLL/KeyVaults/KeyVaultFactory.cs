using System;

namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public class KeyVaultFactory : IKeyVaultFactory
    {
        public IKeyVault Create(AuthenticationType authenticationType, string baseUri, string clientId,
            string authentication)
        {
            switch (authenticationType)
            {
                case AuthenticationType.Certificate:
                    return new CertificateKeyVault(baseUri, clientId, authentication);

                case AuthenticationType.Secret:
                    return new ClientSecretKeyVault(baseUri, clientId, authentication);

                default: throw new InvalidOperationException("AuthenticationType is not supported.");
            }
        }
    }
}