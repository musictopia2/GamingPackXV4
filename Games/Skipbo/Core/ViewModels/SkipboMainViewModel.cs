namespace Skipbo.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class SkipboMainViewModel(CommandContainer commandContainer,
        SkipboMainGameClass mainGame,
        SkipboVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        SkipboGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<SkipboCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.PlayerHand1.Maximum = 5;
        gameContainer.LoadPlayerPiles = LoadPlayerPiles;
        base.AdditionalInitialize();
    }
    protected override async Task OpenAsync()
    {
        LoadPlayerPiles();
    }
    protected override Task CloseAsync()
    {
        if (PlayerPilesScreen != null)
        {
            PlayerPilesScreen = null;
        }
        return base.CloseAsync();
    }
    
    private void LoadPlayerPiles()
    {
        PlayerPilesScreen = Resolver.Resolve<PlayerPilesViewModel>();

    }
    [StateProperty]
    public partial PlayerPilesViewModel? PlayerPilesScreen { get; set; }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    protected override async Task BeforeUnselectCardFromHandAsync()
    {
        mainGame!.UnselectAllCards();
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}