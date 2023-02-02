namespace MultiplayerGamesBlazorLoaderLibrary;
public class HomeSignalREndPoint : ISignalRInfo
{
    Task<string> ISignalRInfo.GetEndPointAsync()
    {
        return Task.FromResult("/hubs/gamepackage/messages"); //this is if using the public nuget packages for your server.
    }
    Task<string> ITCPInfo.GetIPAddressAsync()
    {
        return Task.FromResult(vv1.HomeIPAddress);
    }
    Task<int> ITCPInfo.GetPortAsync()
    {
        return Task.FromResult(vv1.HomePort);
    }
    Task<bool> ISignalRInfo.IsAzureAsync()
    {
        return Task.FromResult(false); //not azure for sure.
    }
}