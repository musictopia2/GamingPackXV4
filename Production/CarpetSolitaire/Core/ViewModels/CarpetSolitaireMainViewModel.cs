namespace CarpetSolitaire.Core.ViewModels;
[InstanceGame]
public partial class CarpetSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<CarpetSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<CarpetSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<CarpetSolitaireMainGameClass>();
    }
}