namespace FreeCellSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class FreeCellSolitaireMainViewModel : SolitaireMainViewModel<FreeCellSolitaireSaveInfo>
{
    public FreePiles FreePiles1;
    public FreeCellSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
        FreePiles1 = new FreePiles(command);
        FreePiles1.PileClickedAsync += FreePiles1_PileClickedAsync;
    }
    private FreeCellSolitaireMainGameClass? _mainGame;
    protected override SolitaireGameClass<FreeCellSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        _mainGame = resolver.ReplaceObject<FreeCellSolitaireMainGameClass>();
        return _mainGame;
    }
    private Task FreePiles1_PileClickedAsync(int Index, BasicPileInfo<SolitaireCard> ThisPile)
    {
        _mainGame!.FreeSelected(Index);
        return Task.CompletedTask;
    }
}