namespace Backgammon.Core.Data;
[SingletonGame]
public class BackgammonDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Backgammon";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 2;
    bool IGameInfo.CanAutoSave => true; //try to allow for autoresume since painting issues would not happen.
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice;
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape;
}