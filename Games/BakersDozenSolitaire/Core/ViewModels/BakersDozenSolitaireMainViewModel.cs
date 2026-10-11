namespace BakersDozenSolitaire.Core.ViewModels;
[InstanceGame]
public partial class BakersDozenSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<BakersDozenSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<BakersDozenSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<BakersDozenSolitaireMainGameClass>();
    }
}