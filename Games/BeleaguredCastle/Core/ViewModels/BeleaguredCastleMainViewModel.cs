namespace BeleaguredCastle.Core.ViewModels;
[InstanceGame]
public partial class BeleaguredCastleMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<BeleaguredCastleSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<BeleaguredCastleSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<BeleaguredCastleMainGameClass>();
    }
}