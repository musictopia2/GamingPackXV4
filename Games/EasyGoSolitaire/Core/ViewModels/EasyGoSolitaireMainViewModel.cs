namespace EasyGoSolitaire.Core.ViewModels;
[InstanceGame]
public partial class EasyGoSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<EasyGoSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<EasyGoSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<EasyGoSolitaireMainGameClass>();
    }
}