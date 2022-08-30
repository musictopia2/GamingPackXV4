namespace DummyRummy.Core.Data;
[SingletonGame]
public class DummyRummySaveInfo : BasicSavedCardClass<DummyRummyPlayerItem, RegularRummyCard>, IMappable, ISaveInfo
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
    private DummyRummyVMData? _model;
    internal void LoadMod(DummyRummyVMData model)
    {
        _model = model;
        _model.UpTo = UpTo;
    }
    public int PlayerWentOut { get; set; }
    public bool SetsCreated { get; set; }
    public int PointsObtained { get; set; }
    public BasicList<SavedSet> SetList { get; set; } = new();
}