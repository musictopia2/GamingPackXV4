namespace TileRummy.Core.Data;
[SingletonGame]
public class TileRummyDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //thought it was rounds but ended up being new game after all.
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers; //if you can have extra computer players, here.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //if not pass and play, then put computer only.
    public const EnumPlayerType PlayerType = EnumPlayerType.NetworkOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Tile Rummy";
    string IGameInfo.GameName => GameName; //put in the name of the game here.
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 4;
    int IGameInfo.MaxPlayers => MaxPlayers; //if different, put here.
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}