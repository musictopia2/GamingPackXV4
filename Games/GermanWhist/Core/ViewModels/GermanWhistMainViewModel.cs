namespace GermanWhist.Core.ViewModels;
[InstanceGame]
public class GermanWhistMainViewModel(CommandContainer commandContainer,
        GermanWhistMainGameClass mainGame,
        GermanWhistVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<GermanWhistCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    private readonly GermanWhistVMData _model = viewModel;
    protected override void AdditionalInitialize()
    {
        _model.Deck1.NeverAutoDisable = true;
        _model.PlayerHand1.Maximum = 13;
        _model.Deck1.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
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
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}