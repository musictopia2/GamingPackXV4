namespace PassOutDiceGame.Core.Data;
[SingletonGame]
public class PassOutDiceGameSaveInfo : BasicSavedBoardDiceGameClass<PassOutDiceGamePlayerItem>, IMappable, ISaveInfo
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
    private PassOutDiceGameVMData? _model;
    internal void LoadMod(PassOutDiceGameVMData model)
    {
        _model = model;
        _model.Instructions = Instructions;
    }
    public int PreviousSpace { get; set; }
    public BasicList<int> SpacePlayers { get; set; } = new();
    public bool DidRoll { get; set; }
}