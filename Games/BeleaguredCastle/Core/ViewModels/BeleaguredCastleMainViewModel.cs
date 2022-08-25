namespace BeleaguredCastle.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class BeleaguredCastleMainViewModel : SolitaireMainViewModel<BeleaguredCastleSaveInfo>
{
    public BeleaguredCastleMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<BeleaguredCastleSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<BeleaguredCastleMainGameClass>();
    }
    //anything else needed is here.
}