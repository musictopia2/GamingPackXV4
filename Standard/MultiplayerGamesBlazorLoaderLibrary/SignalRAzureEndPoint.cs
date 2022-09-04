namespace MultiplayerGamesBlazorLoaderLibrary;
public class SignalRAzureEndPoint : ISignalRInfo
{
    Task<string> ISignalRInfo.GetEndPointAsync()
    {
        return Task.FromResult("/hubs/gamepackage/messages");
    }
    Task<string> ITCPInfo.GetIPAddressAsync()
    {
        if (GlobalDataModel.DataContext == null)
        {
            throw new CustomBasicException("There is no global setting.  Must have global settings in order to get the ipaddress for the signal r service");
        }
        return Task.FromResult(GlobalDataModel.DataContext.GetEndPoint());
    }
    Task<int> ITCPInfo.GetPortAsync()
    {
        return Task.FromResult(80);
    }
    Task<bool> ISignalRInfo.IsAzureAsync()
    {
        return Task.FromResult(true);
    }
}