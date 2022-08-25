namespace SolitaireBoardGame.Core.Data;
[SingletonGame]
public class SolitaireBoardGameDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;
    string IGameInfo.GameName => "Solitaire Board Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 0;
    int IGameInfo.MaxPlayers => 0;
    bool IGameInfo.CanAutoSave => false; //decided no more autoresume on this one.  somehow showed everything as blank (wrong).  not important on this game anyways.
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //default to portrait but can change to what is needed.
}