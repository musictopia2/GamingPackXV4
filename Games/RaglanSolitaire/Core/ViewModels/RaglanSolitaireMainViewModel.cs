namespace RaglanSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class RaglanSolitaireMainViewModel : SolitaireMainViewModel<RaglanSolitaireSaveInfo>
{
    public RaglanSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
        Stock1 = new(command);
        Stock1.Maximum = 6;
        Stock1.Text = "Stock";
        GlobalClass.Stock = Stock1;
    }
    public HandObservable<SolitaireCard>? Stock1;
    protected override SolitaireGameClass<RaglanSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<RaglanSolitaireMainGameClass>();
    }
}