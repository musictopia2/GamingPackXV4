namespace Countdown.Core.Data;
[SingletonGame]
public class CountdownSaveInfo : BasicSavedDiceClass<CountdownDice, CountdownPlayerItem>, IMappable, ISaveInfo
{
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
    public BasicList<int> HintList { get; set; } = new();
    private CountdownVMData? _model;
    internal void LoadMod(CountdownVMData model)
    {
        _model = model;
        _model.Round = Round;
    }
}