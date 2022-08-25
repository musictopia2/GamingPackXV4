namespace KlondikeSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class KlondikeSolitaireMainViewModel : SolitaireMainViewModel<KlondikeSolitaireSaveInfo>
{
    public KlondikeSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<KlondikeSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<KlondikeSolitaireMainGameClass>();
    }
    //anything else needed is here.
}