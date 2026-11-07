namespace MultiplayerGamesBlazorLoaderLibrary;
public static class HostedServerConfiguration
{
    /// <summary>
    /// The default SignalR server base URL, including http:// or https://.
    /// May include an explicit port, for example:
    /// http://192.168.0.150:18010
    /// This supports local or temporary test servers without enabling
    /// the Home Hosting option. Do not include the hub endpoint path.
    /// </summary>
    public static string DefaultServerUrl { get; set; } = "";
}