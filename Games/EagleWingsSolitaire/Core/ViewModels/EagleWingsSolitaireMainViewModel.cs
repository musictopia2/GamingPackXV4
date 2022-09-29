namespace EagleWingsSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class EagleWingsSolitaireMainViewModel : SolitaireMainViewModel<EagleWingsSolitaireSaveInfo>
{
    public DeckObservablePile<SolitaireCard> Heel1;
    public EagleWingsSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
        GlobalClass.MainModel = this;
        Heel1 = new DeckObservablePile<SolitaireCard>(command);
        Heel1.DeckClickedAsync = Heel1_DeckClickedAsync;
        Heel1.SendEnableProcesses(this, () => Heel1.CardsLeft() == 1);
    }
    private EagleWingsSolitaireMainGameClass? _mainGame;
    private async Task Heel1_DeckClickedAsync()
    {
        Heel1.IsSelected = true;
        await _mainGame!.HeelToMainAsync();
    }
    protected override SolitaireGameClass<EagleWingsSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        _mainGame = resolver.ReplaceObject<EagleWingsSolitaireMainGameClass>();
        return _mainGame;
    }
    [LabelColumn]
    public int StartingNumber { get; set; }
    protected override void CommandExecutingChanged()
    {
        StartingNumber = MainPiles1!.StartNumber();
    }
}