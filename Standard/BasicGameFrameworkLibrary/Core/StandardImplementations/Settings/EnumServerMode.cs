namespace BasicGameFrameworkLibrary.Core.StandardImplementations.Settings;
public enum EnumServerMode
{
    HomeHosting, //home network.
    AzureHosting, //azure.
    LocalHosting, //this is focused on others.  so if others want to create a local server, they can.  in this case, no signal r and no https but instead use http.
    MobileServer //this is intended to allow connecting to mobile device.
    //with local hosting and mobile server, requires local ip address to connect to.  the difference is mobile server is only used so the mobile user can start the server.
    //everybody else would use local hosting to connect to that server.  mobile server is iffy if it needs ip address. (probably).
}
