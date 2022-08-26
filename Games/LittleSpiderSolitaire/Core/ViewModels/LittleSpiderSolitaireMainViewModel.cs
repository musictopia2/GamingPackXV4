namespace LittleSpiderSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class LittleSpiderSolitaireMainViewModel : SolitaireMainViewModel<LittleSpiderSolitaireSaveInfo>
{
    public LittleSpiderSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<LittleSpiderSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<LittleSpiderSolitaireMainGameClass>();
    }
    //anything else needed is here.
}