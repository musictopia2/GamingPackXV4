namespace Risk.Core.Data;
[SingletonGame]
public class RiskSaveInfo : BasicSavedGameClass<RiskPlayerItem>, ISavedCardList<RiskCardInfo>, IMappable, ISaveInfo
{
    private RiskVMData? _model;
    private int _armiesToPlace;
    public int ArmiesToPlace
    {
        get => _armiesToPlace;
        set
        {
            if (SetProperty(ref _armiesToPlace, value))
            {
                if (_model is not null)
                {
                    _model.ArmiesToPlace = value;
                }
            }
        }
    }
    private int _bonusReenforcements;
    public int BonusReenforcements
    {
        get => _bonusReenforcements;
        set
        {
            if (SetProperty(ref _bonusReenforcements, value))
            {
                if (_model is not null)
                {
                    _model.BonusReenforcements = value;
                }
            }
        }
    }
    private string _instructions = "";
    public string Instructions
    {
        get { return _instructions; }
        set
        {
            if (SetProperty(ref _instructions, value))
            {
                if (_model != null)
                {
                    _model.Instructions = value;
                }
            }
        }
    }
    internal void LoadMod(RiskVMData model)
    {
        _model = model;
        _model.Instructions = Instructions;
        _model.ArmiesToPlace = ArmiesToPlace;
        _model.BonusReenforcements = BonusReenforcements;
    }
    public BasicList<TerritoryModel> TerritoryList { get; set; } = new();
    public int ArmiesInBattle { get; set; }
    public int NumberDefenseArmies { get; set; }
    public int PreviousTerritory { get; set; }
    public int CurrentTerritory { get; set; }
    public bool ConqueredOne { get; set; }
    public EnumStageList Stage { get; set; }
    public int SetsReturned { get; set; }
    public DeckRegularDict<RiskCardInfo> CardList { get; set; } = new();
    //looks like we need a way to ignore certain properties from my custom serialization system.
    //could make them fields.  however, i think that ignoring makes more sense.

    [JsonIgnore] //this can be ignored in this case.
    public RiskCardInfo? CurrentCard { get; set; }
}