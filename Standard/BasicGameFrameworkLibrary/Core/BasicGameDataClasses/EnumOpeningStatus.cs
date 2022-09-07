namespace BasicGameFrameworkLibrary.Core.BasicGameDataClasses;
public enum EnumOpeningStatus
{
    None,
    HostingWaitingForAtLeastOnePlayer,
    HostingReadyToStart,
    ConnectingWaitingToConnect,
    ConnectedToHost,
    WaitingForHost
}