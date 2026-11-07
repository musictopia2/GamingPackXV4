namespace BeleaguredCastle.Core.Data;
[SingletonGame]
public class BasicData : IGameInfo, ISolitaireData
{
    EnumSolitaireMoveType ISolitaireData.MoveColumns => EnumSolitaireMoveType.OneCardOnly; //this is default.  can change to what i want.
    int ISolitaireData.WasteColumns => 0;
    int ISolitaireData.WasteRows => 0;
    int ISolitaireData.WastePiles => 8; //default is 7.  can change to what it really is.
    int ISolitaireData.Rows => 4;
    int ISolitaireData.Columns => 1;
    bool ISolitaireData.IsKlondike => false;
    int ISolitaireData.CardsNeededWasteBegin => 48;
    int ISolitaireData.CardsNeededMainBegin => 4;
    int ISolitaireData.Deals => -1;
    int ISolitaireData.CardsToDraw => 1;
    bool ISolitaireData.SuitsNeedToMatchForMainPile => true;
    bool ISolitaireData.ShowNextNeededOnMain => false;
    bool ISolitaireData.MainRound => false; //if its round suits; then will be true
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    public const bool CanHaveExtraComputerPlayers = false;
    bool IGameInfo.CanHaveExtraComputerPlayers => CanHaveExtraComputerPlayers;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;
    public const EnumPlayerType PlayerType = EnumPlayerType.SingleOnly;
    EnumPlayerType IGameInfo.PlayerType => PlayerType;
    public const string GameName = "Beleagured Castle";
    string IGameInfo.GameName => GameName; //replace with real name.
    public const int NoPlayers = 0;
    int IGameInfo.NoPlayers => NoPlayers;
    public const int MinPlayers = 0;
    int IGameInfo.MinPlayers => MinPlayers;
    public const int MaxPlayers = 0;
    int IGameInfo.MaxPlayers => MaxPlayers;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = false;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}