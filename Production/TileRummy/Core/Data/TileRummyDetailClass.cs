namespace TileRummy.Core.Data;
[SingletonGame]
public class TileRummyDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //thought it was rounds but ended up being new game after all.
    bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //if not pass and play, then put computer only.
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;
    public const string GameName = "Tile Rummy";
    string IGameInfo.GameName => GameName; //put in the name of the game here.
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 4; //if different, put here.
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}