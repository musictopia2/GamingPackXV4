namespace Minesweeper.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class MinesweeperMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer, ILevelVM
{
    private readonly BasicData _basicData;
    private readonly MinesweeperMainGameClass _mainGame;
    public MinesweeperMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
         LevelClass level,
        BasicData basicData) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        LevelChosen = level.Level; //at this point, can't choose level because its already chosen.
        this.PopulateMinesNeeded();
        _mainGame = resolver.ReplaceObject<MinesweeperMainGameClass>();
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true;
    }
    [LabelColumn]
    public int NumberOfMinesLeft { get; set; }
    [LabelColumn]
    public int HowManyMinesNeeded { get; set; } = 10;
    public int Rows { get; set; } = 9;
    public int Columns { get; set; } = 10;
    [LabelColumn]
    public EnumLevel LevelChosen { get; set; }
    public bool IsFlagging { get; set; }
    [Command(EnumCommandCategory.Plain)]
    public void ChangeFlag()
    {
        IsFlagging = !IsFlagging; //no need for game visible because that is not responsible for it.
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task MakeMoveAsync(MineSquareModel square)
    {
        if (IsFlagging)
        {
            MinesweeperMainGameClass.FlagSingleSquare(square);
            NumberOfMinesLeft = _mainGame.GetMinesLeft();
            return;
        }
        await _mainGame.ClickSingleSquareAsync(square);
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = false; //because no autoresume.
        await base.ActivateAsync();
        NumberOfMinesLeft = _mainGame.NumberOfMines; //i think.
        _mainGame.NumberOfColumns = Columns;
        _mainGame.NumberOfRows = Rows;
        _mainGame.NumberOfMines = HowManyMinesNeeded;
        await _mainGame.NewGameAsync();
    }
}