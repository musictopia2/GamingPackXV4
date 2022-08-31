namespace TeeItUp.Core.Data;
[SingletonGame]
public class TeeItUpSaveInfo : BasicSavedCardClass<TeeItUpPlayerItem, TeeItUpCardInformation>, IMappable, ISaveInfo
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
    public int Begins { get; set; }
    public bool FirstMulligan { get; set; }
    public EnumStatusType GameStatus { get; set; }
    private TeeItUpVMData? _model;
    public void LoadMod(TeeItUpVMData model)
    {
        _model = model;
        _model.Round = Round;
    }
}