namespace ClueBoardGame.Core.Data;
[SingletonGame]
public class ClueBoardGameDetailClass : IGameInfo, IPlayerNeeds
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //computer ai had too many problems now.
    public const string GameName = "Clue Board Game";
    string IGameInfo.GameName => GameName;
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true; //attempt to make it where i have autoresume.  since clues are given and saved.
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
    int IPlayerNeeds.PlayersNeeded => 6;
}