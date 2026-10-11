namespace BisleySolitaire.Core.ViewModels;
[InstanceGame]
public partial class BisleySolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<BisleySolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<BisleySolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<BisleySolitaireMainGameClass>();
    }
}