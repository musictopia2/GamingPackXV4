namespace RummyDice.Core.Data;
[SingletonGame]
public class RummyDiceDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.
    bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly; //if not pass and play, then put computer only.
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;
    public const string GameName = "Rummy Dice";
    string IGameInfo.GameName => GameName; //put in the name of the game here.
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 2;
    int IGameInfo.MaxPlayers => 8; //if different, put here.
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}