namespace RageCardGame.Core.ViewModels;
[InstanceGame]
public class RageCardGameMainViewModel : BasicCardGamesVM<RageCardGameCardInformation>
{
    private readonly RageCardGameVMData _model;
    public RageCardGameMainViewModel(CommandContainer commandContainer,
        RageCardGameMainGameClass mainGame,
        RageCardGameVMData viewModel,
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