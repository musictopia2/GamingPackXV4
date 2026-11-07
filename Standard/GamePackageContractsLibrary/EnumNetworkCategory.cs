namespace GamePackageContractsLibrary;
public enum EnumNetworkCategory
{
    None, Hosting, Client, CloseAll, Message, Error, 
    WaitingForHost, WaitingForGame, GameState, EndGameEarly, 
    RestoreForReconnection, NewGame, WaitingForNextGame, JoinRejected,
    ReturningHost
}