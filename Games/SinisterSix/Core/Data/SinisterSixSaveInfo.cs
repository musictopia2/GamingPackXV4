namespace SinisterSix.Core.Data;
[SingletonGame]
public class SinisterSixSaveInfo : BasicSavedDiceClass<EightSidedDice, SinisterSixPlayerItem>, IMappable, ISaveInfo
{
    private int _maxRolls;
    public int MaxRolls
    {
        get { return _maxRolls; }
        set
        {
            if (SetProperty(ref _maxRolls, value))
            {
                if (_model == null)
                {
                    return;
                }
                _model.MaxRolls = value;
            }
        }
    }
    private SinisterSixVMData? _model;
    internal void LoadMod(SinisterSixVMData model)
    {
        _model = model;
        _model.MaxRolls = MaxRolls;
    }
}