namespace BasicGameFrameworkLibrary.Core.StandardImplementations.Settings;
public class GlobalDataModel
{
    //well see what we do about native support since we can now support desktop natively.
    //public bool FastAnimation { get; set; }
    public string NickName { get; set; } = ""; //decided class because it does need to change this time.
    public string CustomAzureEndPoint { get; set; } = "";
    public EnumAzureMode ServerMode = EnumAzureMode.Private; //decided to change default to private since most people who would know about this would connect only to ones allowed to host.
    private const string _defaultPrivateEndPoint = "https://onlinegameserver.azurewebsites.net/"; //has to keep 
    private const string _defaultPublicEndPoint = "https://gamingpackxpublicserver.azurewebsites.net";
    public string GetEndPoint()
    {
        if (ServerMode == EnumAzureMode.Custom && CustomAzureEndPoint == "")
        {
            //if a person enters wrong address, they have to refresh and choose other options.
            return _defaultPublicEndPoint; //default to public if choosing custom but does not exist.
        }
        return ServerMode switch
        {
            EnumAzureMode.Custom => CustomAzureEndPoint,
            EnumAzureMode.Private => _defaultPrivateEndPoint,
            _ => _defaultPublicEndPoint,
        };
    }
    public static string LocalStorageKey => "settingsv2";
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