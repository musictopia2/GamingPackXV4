namespace GrandfathersClock.Core.ViewModels;
[InstanceGame]
public partial class GrandfathersClockMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<GrandfathersClockSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<GrandfathersClockSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<GrandfathersClockMainGameClass>();
    }
}