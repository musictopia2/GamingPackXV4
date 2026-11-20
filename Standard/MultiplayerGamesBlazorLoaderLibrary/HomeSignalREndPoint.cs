namespace MultiplayerGamesBlazorLoaderLibrary;
public class HomeSignalREndPoint : ISignalRInfo
{
    Task<string> ISignalRInfo.GetEndPointAsync()
    {
        return Task.FromResult("/hubs/gamepackage/messages"); //this is if using the public nuget packages for your server.
    }
    Task<string> ISignalRInfo.GetIPAddressAsync()
    {
        return Task.FromResult(HomeNetworkConfiguration.ActiveBrowserHomeServerUrl
            ?? HomeNetworkConfiguration.HomeIPAddress);
    }
    Task<int?> ISignalRInfo.GetPortAsync()
    {
        int? port = HomeNetworkConfiguration.ActiveBrowserHomeServerUrl is null
            ? HomeNetworkConfiguration.HomePort
            : null;
        return Task.FromResult(port);
    }
}