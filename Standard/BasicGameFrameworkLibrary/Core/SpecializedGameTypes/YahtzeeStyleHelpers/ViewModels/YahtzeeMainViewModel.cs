namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;

public partial class YahtzeeMainViewModel<D> : DiceGamesVM<D>
    where D : SimpleDice, new()
{
    public YahtzeeVMData<D> VMData { get; set; }
    public YahtzeeMainViewModel(
        CommandContainer commandContainer,
        IHoldUnholdProcesses mainGame,
        YahtzeeVMData<D> viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses rollProcesses,
        YahtzeeGameContainer<D> gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, rollProcesses, aggregator)
    {
        _resolver = resolver;
        _gameContainer = gameContainer;
        _toast = toast;
        _gameContainer.GetNewScoreAsync = LoadNewScoreAsync;
        VMData = viewModel;
        IBasicDiceGamesData<D>.NeedsRollIncrement = false;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        await LoadNewScoreAsync();
    }
    public YahtzeeScoresheetViewModel<D>? CurrentScoresheet { get; set; }
    private async Task LoadNewScoreAsync()
    {
        await ClosePossibleScoresheetAsync();
        CurrentScoresheet = _resolver.Resolve<YahtzeeScoresheetViewModel<D>>();
        await LoadScreenAsync(CurrentScoresheet);
    }
    private async Task ClosePossibleScoresheetAsync()
    {
        if (CurrentScoresheet != null)
        {
            await CloseSpecificChildAsync(CurrentScoresheet);
        }
    }
    protected override async Task TryCloseAsync()
    {
        await ClosePossibleScoresheetAsync();
        await base.TryCloseAsync();
    }
    public override bool CanRollDice()
    {
        return VMData.RollNumber <= 3; //iffy now.
    }
    protected override bool CanEnableDice()
    {
        return CanRollDice();
    }
    private readonly IGamePackageResolver _resolver;
    private readonly YahtzeeGameContainer<D> _gameContainer;
    private readonly IToast _toast;

    public PlayerCollection<YahtzeePlayerItem<D>> PlayerList => _gameContainer.PlayerList!;
    public DiceCup<D> GetCup => VMData.Cup!;
}