namespace DiceBoardGamesMultiplayer.Core.Data;
[SingletonGame]
public class DiceBoardGamesMultiplayerSaveInfo : BasicSavedBoardDiceGameClass<DiceBoardGamesMultiplayerPlayerItem>, IMappable, ISaveInfo
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
    private DiceBoardGamesMultiplayerVMData? _model;
    internal void LoadMod(DiceBoardGamesMultiplayerVMData model)
    {
        _model = model;
        _model.Instructions = Instructions;
    }
}