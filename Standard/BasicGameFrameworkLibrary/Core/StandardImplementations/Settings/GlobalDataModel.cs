namespace BasicGameFrameworkLibrary.Core.StandardImplementations.Settings;
public class GlobalDataModel
{

    public string NickName
    {
        get => field;
        set => field = (value ?? "").Replace(" ", "").ToLowerInvariant();
    } = "";

    //public string NickName { get; set; } = "";
    public string CustomHostedServerUrl { get; set; } = "";
    public string HostIPAddress { get; set; } = "";
    public EnumPlayerMode PlayerMode { get; set; } = EnumPlayerMode.None; //will be forced to choose option.
    public EnumServerMode ServerMode { get; set; } = EnumServerMode.HostedServer; //default to hosted server
    public static string LocalStorageKey => "settingsv4"; //change to v4 since there are now breaking changes.
    //maybe not needed anymore because i am doing custom.  this means can be serialized a custom way now.
    //[JsonIgnore] //just to make sure this gets ignored.
    public static GlobalDataModel? DataContext { get; set; }
    public static bool NickNameAcceptable()
    {
        if (DataContext == null)
        {
            throw new CustomBasicException("Settings data failed to load.");
        }

        string name = DataContext.NickName;

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        foreach (char character in name)
        {
            if (character < 'a' || character > 'z')
            {
                return false;
            }
        }

        return true;
    }
}