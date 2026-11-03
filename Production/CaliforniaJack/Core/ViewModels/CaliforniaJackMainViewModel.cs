namespace CaliforniaJack.Core.ViewModels;
[InstanceGame]
public class CaliforniaJackMainViewModel(CommandContainer commandContainer,
        CaliforniaJackMainGameClass mainGame,
        CaliforniaJackVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<CaliforniaJackCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    private CaliforniaJackVMData Model => viewModel;
    protected override void AdditionalInitialize()
    {
        Model.Deck1.NeverAutoDisable = true;
        Model.PlayerHand1!.Maximum = 6;
        Model.Deck1!.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
        base.AdditionalInitialize();
    }
    protected override bool CanEnableDeck()
    {
        return false; //otherwise, can't compile.
    }
    protected override bool CanEnablePile1()
    {
        return false; //otherwise, can't compile.
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}