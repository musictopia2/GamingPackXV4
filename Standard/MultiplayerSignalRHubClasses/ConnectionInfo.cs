namespace MultiplayerSignalRHubClasses;
public class ConnectionInfo
{
    public string ConnectionID { get; set; } = "";
    public string UserID { get; set; } = "";
    public string GameName { get; set; } = "";
    public bool IsConnected { get; set; }
    // A server connection can be waiting without belonging to the hosted game.
    public bool IsAdmitted { get; set; }
    public long WaitingOrder { get; set; }
}