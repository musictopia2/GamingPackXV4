namespace ClueBoardGame.Core.Data;
[SingletonGame]
public class ClueBoardGameDetailClass : IGameInfo, IPlayerNeeds
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //computer ai had too many problems now.
    string IGameInfo.GameName => "Clue Board Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => true; //attempt to make it where i have autoresume.  since clues are given and saved.
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.LargeDevices;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait;
    int IPlayerNeeds.PlayersNeeded => 6;
}