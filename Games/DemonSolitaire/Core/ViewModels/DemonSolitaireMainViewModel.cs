namespace DemonSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class DemonSolitaireMainViewModel : SolitaireMainViewModel<DemonSolitaireSaveInfo>
{
    public DemonSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
        GlobalClass.MainModel = this;
        Heel1 = new DeckObservablePile<SolitaireCard>(command);
        Heel1.SendEnableProcesses(this, () => false);
        Heel1.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
    }
    [LabelColumn]
    public int StartingNumber { get; set; }
    protected override void CommandExecutingChanged()
    {
        StartingNumber = MainPiles1!.StartNumber();

    }
    public DeckObservablePile<SolitaireCard> Heel1;
    protected override SolitaireGameClass<DemonSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<DemonSolitaireMainGameClass>();
    }
}