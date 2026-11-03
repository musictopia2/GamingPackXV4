namespace CalculationSolitaire.Core.Data;
[SingletonGame]
public class BasicData : IGameInfo, ISolitaireData
{
    EnumSolitaireMoveType ISolitaireData.MoveColumns => EnumSolitaireMoveType.CantMove; //this is default.  can change to what i want.
    int ISolitaireData.WasteColumns => 0;
    int ISolitaireData.WasteRows => 0;
    int ISolitaireData.WastePiles => 4; //default is 7.  can change to what it really is.
    int ISolitaireData.Rows => 1;
    int ISolitaireData.Columns => 4;
    bool ISolitaireData.IsKlondike => false;
    int ISolitaireData.CardsNeededWasteBegin => 0;
    int ISolitaireData.CardsNeededMainBegin => 4;
    int ISolitaireData.Deals => 1;
    int ISolitaireData.CardsToDraw => 1;
    bool ISolitaireData.SuitsNeedToMatchForMainPile => false;
    bool ISolitaireData.ShowNextNeededOnMain => true;
    bool ISolitaireData.MainRound => false; //if its round suits; then will be true
    EnumGameType IGameInfo.GameType => EnumGameType.NewGame;
    bool IGameInfo.CanHaveExtraComputerPlayers => false;
    EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.Solitaire;
    EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleOnly;
    public const string GameName = "Calculation Solitaire";
    string IGameInfo.GameName => GameName; //replace with real name.
    int IGameInfo.NoPlayers => 0;
    int IGameInfo.MinPlayers => 0;
    int IGameInfo.MaxPlayers => 0;
    bool IGameInfo.CanAutoSave => true;
    public const bool SupportsCompactLayout = true;
    bool IGameInfo.SupportsCompactLayout => SupportsCompactLayout;
}