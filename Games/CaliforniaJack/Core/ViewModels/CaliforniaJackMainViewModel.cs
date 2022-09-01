namespace CaliforniaJack.Core.ViewModels;
[InstanceGame]
public class CaliforniaJackMainViewModel : BasicCardGamesVM<CaliforniaJackCardInformation>
{
    private readonly CaliforniaJackVMData _model;
    public CaliforniaJackMainViewModel(CommandContainer commandContainer,
        CaliforniaJackMainGameClass mainGame,
        CaliforniaJackVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _model = viewModel;
        _model.Deck1.NeverAutoDisable = true;
        _model.PlayerHand1!.Maximum = 6;
        _model.Deck1!.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
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