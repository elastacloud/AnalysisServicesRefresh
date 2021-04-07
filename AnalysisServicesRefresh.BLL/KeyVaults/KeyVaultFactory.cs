using System;

namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public class KeyVaultFactory : IKeyVaultFactory
    {
        public IKeyVault Create(KeyVaultAuthenticationType authenticationType, string baseUri, string clientId,
            string authentication)
        {
            switch (authenticationType)
            {
                case KeyVaultAuthenticationType.Certificate:
                    return new CertificateKeyVault(baseUri, clientId, authentication);

                case KeyVaultAuthenticationType.Secret:
                    return new ClientSecretKeyVault(baseUri, clientId, authentication);

                default: throw new InvalidOperationException("KeyVaultAuthenticationType is not supported.");
            }
        }
    }
}