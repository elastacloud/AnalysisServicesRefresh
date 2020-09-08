namespace AnalysisServicesRefresh.BLL.KeyVaults
{
    public interface IKeyVaultFactory
    {
        IKeyVault Create(AuthenticationType authenticationType, string baseUri, string clientId, string authentication);
    }
}