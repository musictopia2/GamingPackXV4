namespace MonopolyDicedGame.Core.Data;
[SingletonGame]
public class MonopolyDicedGameSaveInfo : BasicSavedGameClass<MonopolyDicedGamePlayerItem>, IMappable, ISaveInfo
{
    readonly MonopolyDicedGameVMData _model;
    public MonopolyDicedGameSaveInfo()
    {
        _model = aa1.Resolver!.Resolve<MonopolyDicedGameVMData>();
    }
    public int NumberOfHouses { get; set; }
    public bool HasHotel { get; set; } //if you have all 4 houses, then will be hotel.
    public int NumberOfCops { get; set; }
    
    //hint:  when you click on utility, then if you select both, will place automatically.
    //if you select one of them, will place them automatically.
    //however, if you click on chance plus 2 utilities, then display toast error.
    //because too many.

    public BasicList<OwnedModel> Owns { get; set; } = [];
    private int _currentScore;
    public int CurrentScore
    {
        get => _currentScore;
        set
        {
            if (SetProperty(ref _currentScore, value))
            {
                _model.CurrentScore = value;
            }
        }
    }
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