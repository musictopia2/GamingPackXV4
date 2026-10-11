namespace Flinch.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class FlinchMainViewModel(CommandContainer commandContainer,
        FlinchMainGameClass mainGame,
        FlinchVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        FlinchGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<FlinchCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.PlayerHand1.Maximum = 5;
        gameContainer.LoadPlayerPiles = LoadPlayerPiles;
        base.AdditionalInitialize();
    }
    private void LoadPlayerPiles()
    {
        PlayerPilesScreen = Resolver.Resolve<PlayerPilesViewModel>();
    }
    protected override Task CloseAsync()
    {
        PlayerPilesScreen ??= null;
        return base.CloseAsync();
    }
    protected override Task OpenAsync()
    {
        LoadPlayerPiles();
        return base.OpenAsync();
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
    public override bool CanEndTurn()
    {
        return mainGame!.SaveRoot!.GameStatus == EnumStatusList.FirstOne && mainGame.SaveRoot.PlayerFound == 0;
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