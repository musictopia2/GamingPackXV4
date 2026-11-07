namespace GameLoaderBlazorLibrary;
public static class HomeNetworkConfiguration
{
    public static bool CanUseHome { get; set; }
    public static bool DoUseHome { get; internal set; }
    public static int HomePort { get; set; }
    public static string HomeIPAddress { get; set; } = "";
}