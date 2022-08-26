namespace BakersDozenSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class BakersDozenSolitaireMainViewModel : SolitaireMainViewModel<BakersDozenSolitaireSaveInfo>
{
    public BakersDozenSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<BakersDozenSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<BakersDozenSolitaireMainGameClass>();
    }
    //anything else needed is here.
}