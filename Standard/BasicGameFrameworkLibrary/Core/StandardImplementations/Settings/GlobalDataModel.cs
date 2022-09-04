namespace BasicGameFrameworkLibrary.Core.StandardImplementations.Settings;
public class GlobalDataModel
{
    //well see what we do about native support since we can now support desktop natively.
    //public bool FastAnimation { get; set; }
    public string NickName { get; set; } = ""; //decided class because it does need to change this time.
    public string CustomAzureEndPoint { get; set; } = "";
    public string HostIPAddress { get; set; } = ""; //if entered, then you can connect to another computer directly.
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
            //if a person enters wrong address, they have to refresh and choose other options.
            return _defaultPublicEndPoint; //default to public if choosing custom but does not exist.
        }
        return AzureMode switch
        {
            EnumAzureMode.CustomAzure => CustomAzureEndPoint,
            EnumAzureMode.PrivateAzure => _defaultPrivateEndPoint,
            _ => _defaultPublicEndPoint,
        };
    }
    public static string LocalStorageKey => "settingsv3"; //to not break old stuff.
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