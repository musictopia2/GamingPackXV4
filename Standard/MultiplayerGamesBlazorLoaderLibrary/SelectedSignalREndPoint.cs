namespace MultiplayerGamesBlazorLoaderLibrary;
public class SelectedSignalREndPoint(
    HomeSignalREndPoint home,
    RemoteSignalREndPoint remote) : ISignalRInfo
{
    private ISignalRInfo Current =>
        HomeNetworkConfiguration.DoUseHome ? home : remote;

    public Task<string> GetIPAddressAsync()
    {
        return Current.GetIPAddressAsync();
    }

    public Task<int?> GetPortAsync()
    {
        return Current.GetPortAsync();
    }

    public Task<string> GetEndPointAsync()
    {
        return Current.GetEndPointAsync();
    }
}