namespace Mastermind.Core.ViewModels;
[InstanceGame]
public partial class MastermindMainViewModel : ConductorViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    public MastermindMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        LevelClass level,
        BasicData basicData
        ) : base(aggregator)
    {
        LevelChosen = level.LevelChosen;
        CommandContainer = commandContainer;
        _basicData = basicData;
        _global = resolver.ReplaceObject<GlobalClass>();
        CustomColorClass colorClass = new(_global);
        _mainGame = resolver.ReplaceObject<MastermindMainGameClass>();
        MainBoard = resolver.Resolve<GameBoardViewModel>();
        Color1 = new(commandContainer, colorClass);
        Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
        Color1.IsEnabled = true;
        Color1.ItemClickedAsync = Color1_ItemClickedAsync;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private readonly BasicData _basicData;
    private readonly MastermindMainGameClass _mainGame;
    private readonly GlobalClass _global;
    public int LevelChosen { get; set; }
    public SimpleEnumPickerVM<EnumColorPossibilities> Color1;
    public bool CanGiveUp
    {
        get
        {
            if (_global.Solution == null)
            {
                return false;
            }
            return _global.Solution.Count > 0;
        }
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task GiveUpAsync()
    {
        await _mainGame.GiveUpAsync();
    }
    public bool CanAccept => MainBoard.DidFillInGuess();
    [Command(EnumCommandCategory.Plain)]
    public async Task AcceptAsync()
    {
        await MainBoard.SubmitGuessAsync();
    }
    private Task Color1_ItemClickedAsync(EnumColorPossibilities piece)
    {
        MainBoard.SelectedColorForCurrentGuess(piece);
        return Task.CompletedTask;
    }
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    private GameBoardViewModel MainBoard { get; set; }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = false;
        await base.ActivateAsync();
        await LoadScreenAsync(MainBoard); //i think i forgot this too.
        await _mainGame.NewGameAsync(MainBoard);
        Color1.LoadEntireList();
        CommandContainer.UpdateAll();
    }
}