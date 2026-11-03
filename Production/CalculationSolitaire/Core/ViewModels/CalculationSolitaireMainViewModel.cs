namespace CalculationSolitaire.Core.ViewModels;
[InstanceGame]
public partial class CalculationSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<CalculationSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override void AdditionalInitialize()
    {
        DeckPile.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
    }
    protected override SolitaireGameClass<CalculationSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<CalculationSolitaireMainGameClass>();
    }
}