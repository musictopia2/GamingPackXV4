namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
public partial class YahtzeeMainViewModel<D>(CommandContainer commandContainer,
        IHoldUnholdProcesses mainGame,
        YahtzeeVMData<D> viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses rollProcesses,
        YahtzeeGameContainer<D> gameContainer,
        IEventAggregator aggregator) : DiceGamesVM<D>(commandContainer, mainGame, viewModel, basicData, resolver, rollProcesses, aggregator)
    where D : SimpleDice, new()
{
    public YahtzeeVMData<D> VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        IBasicDiceGamesData<D>.NeedsRollIncrement = false;
        gameContainer.GetNewScoreAsync = LoadNewScoreAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override async Task OpenAsync()
    {
        await LoadNewScoreAsync();
        await base.OpenAsync();
    }
    partial void CreateCommands(CommandContainer command);
    public YahtzeeScoresheetViewModel<D>? CurrentScoresheet { get; set; }


    private Task LoadNewScoreAsync()
    {
        CurrentScoresheet ??=
            Resolver.Resolve<YahtzeeScoresheetViewModel<D>>();

        CommandContainer.UpdateSpecificAction(YahtzeeActions.Scoresheet);
        return Task.CompletedTask;
    }

    //private async Task LoadNewScoreAsync()
    //{
    //    await ClosePossibleScoresheetAsync();
    //    CurrentScoresheet = Resolver.Resolve<YahtzeeScoresheetViewModel<D>>();
    //    CommandContainer.UpdateSpecificAction(YahtzeeActions.Scoresheet);
    //}
    protected override async Task CloseAsync()
    {
        await ClosePossibleScoresheetAsync();
    }
    private async Task ClosePossibleScoresheetAsync()
    {
        CurrentScoresheet?.Dispose();
        CurrentScoresheet = null; //hopefully this stimple.
    }
    public override bool CanRollDice()
    {
        return VMData.RollNumber <= 3;
    }
    protected override bool CanEnableDice()
    {
        return CanRollDice();
    }

    public PlayerCollection<YahtzeePlayerItem<D>> PlayerList => gameContainer.PlayerList!;
    public DiceCup<D> GetCup => VMData.Cup!;
}