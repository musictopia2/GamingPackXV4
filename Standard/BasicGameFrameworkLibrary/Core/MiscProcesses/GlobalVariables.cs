namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public static class GlobalVariables
{
    /// <summary>
    /// if set to true, then whoever does it must figure out their own home network stuff.
    /// </summary>
    public static bool CanUseHome { get; set; }
    public static bool DoUseHome { get; set; } //if home is used, then you have to figure out the home stuff.
    public static int HomePort { get; set; }
    public static string HomeIPAddress { get; set; } = "";
    //anybody who wants to do the custom stuff for home hosting must put in what the port and the ip address is.
    //can't do as interface.  otherwise, source generators can't easily pick this up.
}