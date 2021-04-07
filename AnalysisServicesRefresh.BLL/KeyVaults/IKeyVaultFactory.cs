namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public interface IKeyVaultFactory
    {
        IKeyVault Create(KeyVaultAuthenticationType authenticationType, string baseUri, string clientId, string authentication);
    }
}