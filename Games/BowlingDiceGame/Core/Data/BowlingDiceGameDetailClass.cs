namespace BowlingDiceGame.Core.Data;
[SingletonGame]
public class BowlingDiceGameDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => true;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Either;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    string IGameInfo.GameName => "Bowling Dice Game";
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 6;
    bool IGameInfo.CanAutoSave => false; //somehow autoresume had too many problems now.
    EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //unable to do on phone because of frame details. maybe in new we can do phone (?)
    EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //default to portrait but can change to what is needed.
}