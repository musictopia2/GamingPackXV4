namespace PickelCardGame.Core.ViewModels;
[InstanceGame]
public class PickelCardGameMainViewModel : BasicCardGamesVM<PickelCardGameCardInformation>
{
    private readonly PickelCardGameVMData _model;
    public PickelCardGameMainViewModel(CommandContainer commandContainer,
        PickelCardGameMainGameClass mainGame,
        PickelCardGameVMData viewModel,
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