namespace RummyDice.Core.Data;
[SingletonGame]
public class RummyDiceSaveInfo : BasicSavedGameClass<RummyDicePlayerItem>, IMappable, ISaveInfo
{
    private readonly RummyDiceVMData _model;
    public RummyDiceSaveInfo()
    {
        _model = aa.Resolver!.Resolve<RummyDiceVMData>();
    }
    public BasicList<RummyDiceInfo> DiceList { get; set; } = new();
    private int _rollNumber;
    public int RollNumber
    {
        get { return _rollNumber; }
        set
        {
            if (SetProperty(ref _rollNumber, value))
            {
                _model.RollNumber = value;
            }
        }
    }
    public bool SomeoneFinished { get; set; }
    public BasicList<BasicList<RummyDiceInfo>> TempSets { get; set; } = new();
}