namespace FillOrBust.Core.Data;
[SingletonGame]
public class FillOrBustSaveInfo : BasicSavedCardClass<FillOrBustPlayerItem, FillOrBustCardInformation>, IMappable, ISaveInfo
{
    private FillOrBustVMData? _model;
    public int FillsRequired { get; set; }
    public EnumGameStatusList GameStatus { get; set; }
    private int _tempScore;
    public int TempScore
    {
        get { return _tempScore; }
        set
        {
            if (SetProperty(ref _tempScore, value))
            {
                if (_model != null)
                {
                    _model.TempScore = value;
                }
            }
        }
    }
    private int _diceScore;
    public int DiceScore
    {
        get { return _diceScore; }
        set
        {
            if (SetProperty(ref _diceScore, value))
            {
                if (_model != null)
                {
                    _model.DiceScore = value;
                }
            }
        }
    }
    public DiceList<SimpleDice> DiceList { get; set; } = new();
    public void LoadMod(FillOrBustVMData model)
    {
        _model = model;
        _model.DiceScore = DiceScore;
        _model.TempScore = TempScore;
    }
}