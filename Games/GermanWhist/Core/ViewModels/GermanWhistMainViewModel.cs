namespace GermanWhist.Core.ViewModels;
[InstanceGame]
public class GermanWhistMainViewModel : BasicCardGamesVM<GermanWhistCardInformation>
{
    private readonly GermanWhistVMData _model;
    public GermanWhistMainViewModel(CommandContainer commandContainer,
        GermanWhistMainGameClass mainGame,
        GermanWhistVMData viewModel,
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
        _model.PlayerHand1.Maximum = 13;
        _model.Deck1.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
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