namespace Concentration.Core.ViewModels;
[InstanceGame]
public class ConcentrationMainViewModel : BasicCardGamesVM<RegularSimpleCard>
{
    private readonly ConcentrationMainGameClass _mainGame; //if we don't need, delete.
    public ConcentrationVMData VMData { get; set; }
    public ConcentrationMainViewModel(CommandContainer commandContainer,
        ConcentrationMainGameClass mainGame,
        ConcentrationVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.GameBoard1.PileClickedAsync += GameBoard1_PileClickedAsync;
    }
    protected override Task TryCloseAsync()
    {
        VMData.GameBoard1.PileClickedAsync -= GameBoard1_PileClickedAsync;
        return base.TryCloseAsync();
    }
    private async Task GameBoard1_PileClickedAsync(int index, BasicPileInfo<RegularSimpleCard> thisPile)
    {
        await _mainGame.SelectCardAsync(thisPile.ThisObject.Deck);
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