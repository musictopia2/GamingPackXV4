namespace AgnesSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class AgnesSolitaireMainViewModel : SolitaireMainViewModel<AgnesSolitaireSaveInfo>
{
    public AgnesSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<AgnesSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<AgnesSolitaireMainGameClass>();
    }
    [LabelColumn]
    public int StartingNumber { get; set; }
    protected override void CommandExecutingChanged()
    {
        StartingNumber = MainPiles1!.StartNumber();
    }
}