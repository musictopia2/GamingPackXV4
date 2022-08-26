namespace MarthaSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class MarthaSolitaireMainViewModel : SolitaireMainViewModel<MarthaSolitaireSaveInfo>
{
    public MarthaSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<MarthaSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<MarthaSolitaireMainGameClass>();
    }
    //anything else needed is here.
}