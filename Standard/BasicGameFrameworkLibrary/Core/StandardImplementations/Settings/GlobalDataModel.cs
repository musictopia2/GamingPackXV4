namespace BasicGameFrameworkLibrary.Core.StandardImplementations.Settings;
public class GlobalDataModel
{
    public string NickName { get; set; } = "";
    public string CustomAzureEndPoint { get; set; } = "";
    public string HostIPAddress { get; set; } = "";
    public EnumServerMode ServerMode { get; set; } = EnumServerMode.AzureHosting; //default to azure hosting.
    public EnumAzureMode AzureMode = EnumAzureMode.PrivateAzure; //decided to change default to private since most people who would know about this would connect only to ones allowed to host.
    private const string _defaultPrivateEndPoint = "https://onlinegameserver.azurewebsites.net/"; //has to keep 
    private const string _defaultPublicEndPoint = "https://gamingpackxpublicserver.azurewebsites.net";
    public string GetEndPoint()
    {
        if (ServerMode != EnumServerMode.AzureHosting)
        {
            throw new CustomBasicException("Only azure hosting can get the end point for now");
        }
        if (AzureMode == EnumAzureMode.CustomAzure && CustomAzureEndPoint == "")
        {
            return _defaultPublicEndPoint;
        }
        return AzureMode switch
        {
            EnumAzureMode.CustomAzure => CustomAzureEndPoint,
            EnumAzureMode.PrivateAzure => _defaultPrivateEndPoint,
            _ => _defaultPublicEndPoint,
        };
    }
    public static string LocalStorageKey => "settingsv3";
    //maybe not needed anymore because i am doing custom.  this means can be serialized a custom way now.
    //[JsonIgnore] //just to make sure this gets ignored.
    public static GlobalDataModel? DataContext { get; set; }
    public static bool NickNameAcceptable()
    {
        if (DataContext == null)
        {
            throw new CustomBasicException("Settings data failed to load.");
        }
        return string.IsNullOrWhiteSpace(DataContext.NickName) == false;
    }
}