namespace BasicGameFrameworkLibrary.Core.BasicGameDataClasses;

//for now, still needed.  may rethink later (especially with reconnected games)
public enum EnumOpeningStatus
{
    None,
    HostingWaitingForAtLeastOnePlayer,
    HostingReadyToStart,
    ConnectingWaitingToConnect,
    ConnectedToHost,
    WaitingForHost
}