namespace Bingo.Core.Data;
[SingletonGame]
public class BingoDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.
    public const bool CanHaveExtraComputerPlayers = true;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers; //if you can have extra computer players, here.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //if not pass and play, then put computer only.
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Bingo";
    string IGameInfo.GameName => GameName; //put in the name of the game here.
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 10;
    int IGameInfo.MaxPlayers => MaxPlayers; //has to reduce to 10.  otherwise, can't fit everything.
    bool IGameInfo.CanAutoSave => false; //for sure can't autoresume this game.
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}