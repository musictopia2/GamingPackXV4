namespace CalculationSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class CalculationSolitaireMainViewModel : SolitaireMainViewModel<CalculationSolitaireSaveInfo>
{
    public CalculationSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override Task ActivateAsync()
    {
        DeckPile!.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
        return base.ActivateAsync();
    }
    protected override SolitaireGameClass<CalculationSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<CalculationSolitaireMainGameClass>();
    }
}