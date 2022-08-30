namespace FiveCrowns.Core.Data;
[SingletonGame]
public class FiveCrownsSaveInfo : BasicSavedCardClass<FiveCrownsPlayerItem, FiveCrownsCardInformation>, IMappable, ISaveInfo
{
    private int _upTo;
    public int UpTo
    {
        get { return _upTo; }
        set
        {
            if (SetProperty(ref _upTo, value))
            {
                if (_model == null)
                {
                    return;
                }
                _model.UpTo = UpTo;
            }
        }
    }
    private FiveCrownsVMData? _model;
    internal void LoadMod(FiveCrownsVMData model)
    {
        _model = model;
        _model.UpTo = UpTo;
    }
    public int PlayerWentOut { get; set; }
    public bool SetsCreated { get; set; }
    public BasicList<SavedSet> SetList { get; set; } = new();
}