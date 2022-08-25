namespace SolitaireCardGamesSimple.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class SolitaireCardGamesSimpleMainViewModel : SolitaireMainViewModel<SolitaireCardGamesSimpleSaveInfo>
{
    public SolitaireCardGamesSimpleMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<SolitaireCardGamesSimpleSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<SolitaireCardGamesSimpleMainGameClass>();
    }
    //anything else needed is here.
}