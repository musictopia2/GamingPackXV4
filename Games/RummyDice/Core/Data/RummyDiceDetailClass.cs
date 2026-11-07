namespace RummyDice.Core.Data;
[SingletonGame]
public class RummyDiceDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers; //if you can have extra computer players, here.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly; //if not pass and play, then put computer only.
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleAndNetworked;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Rummy Dice";
    string IGameInfo.GameName => GameName; //put in the name of the game here.
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 2;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 8;
    int IGameInfo.MaxPlayers => MaxPlayers; //if different, put here.
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}