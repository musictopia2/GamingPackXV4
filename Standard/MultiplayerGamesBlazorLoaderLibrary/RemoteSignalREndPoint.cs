namespace MultiplayerGamesBlazorLoaderLibrary;
public class RemoteSignalREndPoint : ISignalRInfo
{
    Task<string> ISignalRInfo.GetEndPointAsync()
    {
        return Task.FromResult("/hubs/gamepackage/messages");
    }
    Task<string> ISignalRInfo.GetIPAddressAsync()
    {
        var settings = GlobalDataModel.DataContext;
        if (settings == null)
        {
            throw new CustomBasicException(
                "Global settings are required to get the SignalR server address");
        }

        string address = string.IsNullOrWhiteSpace(settings.CustomHostedServerUrl)
            ? HostedServerConfiguration.DefaultServerUrl
            : settings.CustomHostedServerUrl;

        return Task.FromResult(address.Trim().TrimEnd('/'));
    }
    Task<int?> ISignalRInfo.GetPortAsync()
    {
        return Task.FromResult<int?>(null);
    }
}