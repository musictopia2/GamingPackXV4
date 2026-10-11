namespace Minesweeper.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class MinesweeperMainViewModel(CommandContainer commandContainer,
        IGamePackageResolver resolver,
         LevelClass level,
        BasicData basicData,
        IEventAggregator aggregator) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>, ILevelVM
{
    private MinesweeperMainGameClass _mainGame = null!;
    private partial void Subscribe();
    private partial void Unsubscribe();
    partial void CreateCommands(CommandContainer container);
    public bool CanEnableBasics()
    {
        return true;
    }
    [LabelColumn]
    public partial int NumberOfMinesLeft { get; set; }
    [LabelColumn]
    public partial int HowManyMinesNeeded { get; set; }
    public int Rows { get; set; } = 9;
    public int Columns { get; set; } = 10;
    [LabelColumn]
    public partial EnumLevel LevelChosen { get; set; }
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
    
    void IMainGameVM.Initialize()
    {
        LevelChosen = level.Level; //at this point, can't choose level because its already chosen.
        this.PopulateMinesNeeded();
        _mainGame = resolver.ReplaceObject<MinesweeperMainGameClass>();
        CreateCommands(commandContainer);
        Subscribe();
    }

    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = false; //because no autoresume.
        NumberOfMinesLeft = _mainGame.NumberOfMines; //i think.
        _mainGame.NumberOfColumns = Columns;
        _mainGame.NumberOfRows = Rows;
        _mainGame.NumberOfMines = HowManyMinesNeeded;
        await _mainGame.NewGameAsync();
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();   
    }
}