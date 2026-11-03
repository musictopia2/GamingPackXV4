namespace MarthaSolitaire.Core.ViewModels;
[InstanceGame]
public partial class MarthaSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<MarthaSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<MarthaSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<MarthaSolitaireMainGameClass>();
    }
}