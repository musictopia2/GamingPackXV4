namespace EasyGoSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class EasyGoSolitaireMainViewModel : SolitaireMainViewModel<EasyGoSolitaireSaveInfo>
{
    public EasyGoSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<EasyGoSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<EasyGoSolitaireMainGameClass>();
    }
    //anything else needed is here.
}