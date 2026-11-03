namespace Mancala.Core.Data;
[SingletonGame]
public class MancalaDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.
    bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly; //if not pass and play, then put computer only.
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "Mancala";
    string IGameInfo.GameName => GameName; //put in the name of the game here.
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2; //if different, put here.
    bool IGameInfo.CanAutoSave => true; //attempt to allow autoresume even for mancala since its getting smart about when to send information for reconnected clients.
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}