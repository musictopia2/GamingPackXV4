namespace GrandfathersClock.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class GrandfathersClockMainViewModel : SolitaireMainViewModel<GrandfathersClockSaveInfo>
{
    public GrandfathersClockMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<GrandfathersClockSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<GrandfathersClockMainGameClass>();
    }
}