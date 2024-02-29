namespace MonopolyDicedGame.Core.Data;
[SingletonGame]
public class MonopolyDicedGameSaveInfo : BasicSavedGameClass<MonopolyDicedGamePlayerItem>, IMappable, ISaveInfo
{
    MonopolyDicedGameVMData _model;
    public MonopolyDicedGameSaveInfo()
    {
        _model = aa1.Resolver!.Resolve<MonopolyDicedGameVMData>();
    }
    public int NumberOfCops { get; set; }
    private int _rollNumber = 1;
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
}