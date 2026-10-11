namespace AgnesSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class AgnesSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver
        ) : SolitaireMainViewModel<AgnesSolitaireSaveInfo>(aggregator, command, resolver)
{
    protected override SolitaireGameClass<AgnesSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<AgnesSolitaireMainGameClass>();
    }
    [LabelColumn]
    public partial int StartingNumber { get; set; }
    protected override void CommandExecutingChanged()
    {
        StartingNumber = MainPiles1!.StartNumber();
    }
}