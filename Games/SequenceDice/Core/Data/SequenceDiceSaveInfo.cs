namespace SequenceDice.Core.Data;
[SingletonGame]
public class SequenceDiceSaveInfo : BasicSavedBoardDiceGameClass<SequenceDicePlayerItem>, IMappable, ISaveInfo
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
    private SequenceDiceVMData? _model;
    internal void LoadMod(SequenceDiceVMData model)
    {
        _model = model;
        _model.Instructions = Instructions;
    }
    public EnumGameStatusList GameStatus { get; set; }
    public SequenceBoardCollection GameBoard { get; set; } = new();
}