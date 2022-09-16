namespace ClueBoardGame.Core.Data;
[SingletonGame]
public class ClueBoardGameSaveInfo : BasicSavedBoardDiceGameClass<ClueBoardGamePlayerItem>, IMappable, ISaveInfo
{
    private string _instructions = "";
    public string Instructions
    {
        get { return _instructions; }
        set
        {
            if (SetProperty(ref _instructions, value))
            {
                //can decide what to do when property changes
                if (_model != null)
                {
                    _model.Instructions = value;
                }
            }
        }
    }
    private ClueBoardGameVMData? _model;
    internal void LoadMod(ClueBoardGameVMData model)
    {
        _model = model;
        _model.Instructions = Instructions;
        _model.LeftToMove = MovesLeft;
        if (CurrentPrediction is not null)
        {
            _model.CurrentCharacterName = CurrentPrediction.CharacterName;
            _model.CurrentWeaponName = CurrentPrediction.WeaponName;
            _model.CurrentRoomName = CurrentPrediction.RoomName;
        }
    }
    public int DiceNumber { get; set; }
    public PredictionInfo? CurrentPrediction { get; set; }
    private int _movesLeft;
    public int MovesLeft
    {
        get { return _movesLeft; }
        set
        {
            if (SetProperty(ref _movesLeft, value))
            {
                if (_model != null)
                {
                    _model.LeftToMove = MovesLeft;
                }
            }
        }
    }
    public bool AccusationMade { get; set; }
    //public bool ShowedMessage { get; set; }
    public Dictionary<int, CharacterInfo> CharacterList { get; set; } = new();
    public PredictionInfo Solution { get; set; } = new();
    public Dictionary<int, MoveInfo> PreviousMoves { get; set; } = new();
    public Dictionary<int, WeaponInfo> WeaponList { get; set; } = new();
    public EnumClueStatusList GameStatus { get; set; }
    public int PreviousClue { get; set; } //this means if given, then whoevers turn it is can show that clue.

}