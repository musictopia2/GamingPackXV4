namespace Concentration.Core.ViewModels;
[InstanceGame]
public class ConcentrationMainViewModel(CommandContainer commandContainer,
        ConcentrationMainGameClass mainGame,
        ConcentrationVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    public ConcentrationVMData VMData { get; set; } = viewModel;
    
    protected override void AdditionalInitialize()
    {
        VMData.Deck1.NeverAutoDisable = true;
        VMData.GameBoard1.PileClickedAsync = GameBoard1_PileClickedAsync;
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        VMData.GameBoard1.PileClickedAsync = null;
        return base.CloseAsync();
    }
    private async Task GameBoard1_PileClickedAsync(int index, BasicPileInfo<RegularSimpleCard> thisPile)
    {
        await mainGame.SelectCardAsync(thisPile.ThisObject.Deck);
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