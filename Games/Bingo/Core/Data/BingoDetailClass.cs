namespace Bingo.Core.Data;
[SingletonGame]
public class BingoDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.
    bool IGameInfo.CanHaveExtraComputerPlayers => true; //if you can have extra computer players, here.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //if not pass and play, then put computer only.
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "Bingo";
    string IGameInfo.GameName => GameName; //put in the name of the game here.
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 10; //has to reduce to 10.  otherwise, can't fit everything.
    bool IGameInfo.CanAutoSave => false; //for sure can't autoresume this game.
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}