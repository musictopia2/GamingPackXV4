namespace CribbagePatience.Core.Data;
[SingletonGame]
public class CribbagePatienceDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;
    string IGameInfo.GameName => "Cribbage Patience";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 0;
    int IGameInfo.MaxPlayers => 0;
    bool IGameInfo.CanAutoSave => false; //can't have autoresume.  otherwise, if you are checking out the scores and close out, it gets hosed.  good news is fast game anyways.
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
}