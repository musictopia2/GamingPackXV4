namespace LittleSpiderSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class LittleSpiderSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<LittleSpiderSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<LittleSpiderSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<LittleSpiderSolitaireMainGameClass>();
    }
    //anything else needed is here.
}