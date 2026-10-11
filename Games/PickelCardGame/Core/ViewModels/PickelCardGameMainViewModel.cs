namespace PickelCardGame.Core.ViewModels;
[InstanceGame]
public class PickelCardGameMainViewModel(CommandContainer commandContainer,
        PickelCardGameMainGameClass mainGame,
        PickelCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<PickelCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        base.AdditionalInitialize();
    }
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
    public override bool CanEnableAlways()
    {
        return true;
    }
}