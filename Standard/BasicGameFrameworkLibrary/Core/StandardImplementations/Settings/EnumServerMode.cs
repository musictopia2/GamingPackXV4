namespace BasicGameFrameworkLibrary.Core.StandardImplementations.Settings;
public enum EnumServerMode
{
    HomeHosting,  //SignalR server with an address and explicit port.
    HostedServer, //Default or custom hosted SignalR server.
    LocalHosting, //Connect using the local networking implementation.
    MobileServer  //Start the server on a mobile device.
}