namespace Aggravation.Core.Data;
[SingletonGame]
public class AggravationSaveInfo : BasicSavedBoardDiceGameClass<AggravationPlayerItem>, IMappable, ISaveInfo
{
    private string _instructions = "";
    public string Instructions
    {
        get { return _instructions; }
        set
        {
            if (SetProperty(ref _instructions, value))
            {
                //can decide what to do when property changes
                if (_model != null)
                {
                    _model.Instructions = value;
                }
            }

        }
    }
    private AggravationVMData? _model;
    internal void LoadMod(AggravationVMData model)
    {
        _model = model;
        _model.Instructions = Instructions;
    }
    public int PreviousSpace { get; set; }
    public BasicList<MoveInfo> MoveList { get; set; } = new();
    public EnumColorChoice OurColor { get; set; }
    public int DiceNumber { get; set; }
}