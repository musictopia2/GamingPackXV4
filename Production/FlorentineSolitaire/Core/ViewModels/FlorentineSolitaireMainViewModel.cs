namespace FlorentineSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class FlorentineSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<FlorentineSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<FlorentineSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<FlorentineSolitaireMainGameClass>();
    }
    [LabelColumn]
    public partial int StartingNumber { get; set; }
    protected override void CommandExecutingChanged()
    {
        StartingNumber = MainPiles1!.StartNumber();
    }
}