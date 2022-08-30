namespace CoveredUp.Core.Data;
[SingletonGame]
public class CoveredUpSaveInfo : BasicSavedCardClass<CoveredUpPlayerItem, RegularSimpleCard>, IMappable, ISaveInfo
{
    private int _round;
    public int Round
    {
        get { return _round; }
        set
        {
            if (SetProperty(ref _round, value))
            {
                if (_model != null)
                {
                    _model.Round = value;
                }
            }
        }
    }
    public int UpTo { get; set; }
    public int Begins { get; set; }
    public bool WentOut { get; set; }
    private CoveredUpVMData? _model;
    public void LoadMod(CoveredUpVMData model)
    {
        _model = model;
        _model.Round = Round;
    }
}