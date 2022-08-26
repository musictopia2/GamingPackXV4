namespace CarpetSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class CarpetSolitaireMainViewModel : SolitaireMainViewModel<CarpetSolitaireSaveInfo>
{
    public CarpetSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<CarpetSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<CarpetSolitaireMainGameClass>();
    }
    //anything else needed is here.
}