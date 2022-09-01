namespace Spades2Player.Core.Data;
[SingletonGame]
public class Spades2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem>, ITrickStatusSavedClass, IMappable, ISaveInfo
{
    public bool FirstCard { get; set; }
    private EnumGameStatus _gameStatus;
    public EnumGameStatus GameStatus
    {
        get { return _gameStatus; }
        set
        {
            if (SetProperty(ref _gameStatus, value))
            {
                if (_model != null)
                {
                    _model.GameStatus = value;
                }
            }
        }
    }
    public SavedDiscardPile<Spades2PlayerCardInformation>? OtherPile { get; set; }
    private int _roundNumber;
    public int RoundNumber
    {
        get { return _roundNumber; }
        set
        {
            if (SetProperty(ref _roundNumber, value))
            {
                if (_model != null)
                {
                    _model.RoundNumber = value;
                }
            }
        }
    }
    private Spades2PlayerVMData? _model;
    public void LoadMod(Spades2PlayerVMData model)
    {
        _model = model;
        _model.RoundNumber = RoundNumber;
        _model.GameStatus = GameStatus;
    }
    public bool NeedsToDraw { get; set; }
    public EnumTrickStatus TrickStatus { get; set; }
}