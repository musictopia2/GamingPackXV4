namespace Risk.Core.Data;
[SingletonGame]
public class RiskDetailClass : IGameInfo, IPlayerNeeds
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //can't be network only.  otherwise, can't choose color
    public const string GameName = "Risk";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int IPlayerNeeds.PlayersNeeded
    {
        get
        {
            return 3; //this means if there is at least 3 players, its okay.
        }
    }
}