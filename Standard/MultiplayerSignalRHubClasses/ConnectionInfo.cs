﻿namespace MultiplayerSignalRHubClasses;
public class ConnectionInfo
{
    public string ConnectionID { get; set; } = "";
    public string UserID { get; set; } = "";
    public string GameName { get; set; } = "";
    public bool IsConnected { get; set; }
}