namespace LifeBoardGame.Core.Data;
[SingletonGame]
public class LifeBoardGameSaveInfo : BasicSavedGameClass<LifeBoardGamePlayerItem>, IMappable, ISaveInfo
{
    private IEventAggregator? _aggregator;
    private EnumWhatStatus _gameStatus;
    public EnumWhatStatus GameStatus
    {
        get { return _gameStatus; }
        set
        {
            if (SetProperty(ref _gameStatus, value))
            {
                if (_model != null)
                {
                    _model.GameStatus = value;
                    if (_aggregator is null)
                    {
                        _aggregator = aa.Resolver!.Resolve<IEventAggregator>();
                    }
                    _aggregator.Publish(GameStatus);
                }
            }
        }
    }
    public BasicList<TileInfo> TileList { get; set; } = new();
    public DeckRegularDict<HouseInfo> HouseList { get; set; } = new();
    public bool WasMarried { get; set; }
    public bool GameStarted { get; set; }
    public bool WasNight { get; set; }
    public int MaxChosen { get; set; }
    public int NewPosition { get; set; }
    public bool EndAfterSalary { get; set; }
    public bool EndAfterStock { get; set; }
    public int NumberRolled { get; set; }
    public int SpinPosition { get; set; }
    public int ChangePosition { get; set; }
    public BasicList<int> SpinList { get; set; } = new(); //needs this so knows for entertainer getting 100,000 dollars.
    public int TemporarySpaceSelected { get; set; }
    private LifeBoardGameVMData? _model;
    internal void LoadMod(LifeBoardGameVMData model)
    {
        _model = model;
        _model.GameStatus = GameStatus;
    }
}