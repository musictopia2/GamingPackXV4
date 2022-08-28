namespace RollEm.Core.Data;
[SingletonGame]
public class RollEmSaveInfo : BasicSavedDiceClass<SimpleDice, RollEmPlayerItem>, IMappable, ISaveInfo
{
    public BasicList<string> SpaceList { get; set; } = new();
    public EnumStatusList GameStatus { get; set; }
    private int _round;
    public int Round
    {
        get { return _round; }
        set
        {
            if (SetProperty(ref _round, value))
            {
                if (_model == null)
                {
                    return;
                }
                _model.Round = value;
            }
        }
    }
    private RollEmVMData? _model;
    internal void LoadMod(RollEmVMData model)
    {
        _model = model;
        _model.Round = Round;
    }
}