namespace FlorentineSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class FlorentineSolitaireMainViewModel : SolitaireMainViewModel<FlorentineSolitaireSaveInfo>
{
    public FlorentineSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<FlorentineSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<FlorentineSolitaireMainGameClass>();
    }
    [LabelColumn]
    public int StartingNumber { get; set; }
    protected override void CommandExecutingChanged()
    {
        StartingNumber = MainPiles1!.StartNumber();
    }
}