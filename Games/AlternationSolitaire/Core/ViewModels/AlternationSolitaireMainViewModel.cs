namespace AlternationSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class AlternationSolitaireMainViewModel : SolitaireMainViewModel<AlternationSolitaireSaveInfo>
{
    public AlternationSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<AlternationSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<AlternationSolitaireMainGameClass>();
    }
    //anything else needed is here.
}