namespace KlondikeSolitaire.Core.ViewModels;
[InstanceGame]
public partial class KlondikeSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<KlondikeSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<KlondikeSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<KlondikeSolitaireMainGameClass>();
    }
}