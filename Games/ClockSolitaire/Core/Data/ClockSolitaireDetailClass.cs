namespace ClockSolitaire.Core.Data;
[SingletonGame]
public class ClockSolitaireDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;
    string IGameInfo.GameName => "Clock Solitaire";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 0;
    int IGameInfo.MaxPlayers => 0;
    bool IGameInfo.CanAutoSave => true;
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //default to portrait but can change to what is needed.
}