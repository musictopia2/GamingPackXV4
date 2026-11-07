namespace ListChooser.Core.Data;
[SingletonGame]
public class ListChooserDetailClass : IGameInfo
{
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "List Chooser";
    string IGameInfo.GameName => GameName;
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 0;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 0;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}