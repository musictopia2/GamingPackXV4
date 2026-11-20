namespace GameLoaderBlazorLibrary;
public static class HomeNetworkConfiguration
{
    // Native apps enable home hosting and supply the address and port.
    public static bool CanUseHome { get; set; }

    internal static string? SavedBrowserHomeServerUrl { get; set; }
    internal static bool HomeHostingAvailable =>
        CanUseHome || SavedBrowserHomeServerUrl is not null;

    // A native app's explicit configuration takes priority. The browser URL
    // already contains the scheme and any port.
    public static string? ActiveBrowserHomeServerUrl =>
        DoUseHome && CanUseHome == false ? SavedBrowserHomeServerUrl : null;
    public static bool DoUseHome { get; internal set; }
    public static int HomePort { get; set; }
    public static string HomeIPAddress { get; set; } = "";
}