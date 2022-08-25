namespace BisleySolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class BisleySolitaireMainViewModel : SolitaireMainViewModel<BisleySolitaireSaveInfo>
{
    public BisleySolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<BisleySolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<BisleySolitaireMainGameClass>();
    }
    //anything else needed is here.
}