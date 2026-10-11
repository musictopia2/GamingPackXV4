namespace SolitaireCardGamesSimple.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class SolitaireCardGamesSimpleMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<SolitaireCardGamesSimpleSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<SolitaireCardGamesSimpleSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<SolitaireCardGamesSimpleMainGameClass>();
    }
    //anything else needed is here.
}