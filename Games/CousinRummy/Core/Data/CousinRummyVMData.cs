namespace CousinRummy.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class CousinRummyVMData : IBasicCardGamesData<RegularRummyCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string PhaseData { get; set; } = "";
    [LabelColumn]
    public string OtherLabel { get; set; } = "";
    public CousinRummyVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new DeckObservablePile<RegularRummyCard>(command);
        Pile1 = new SingleObservablePile<RegularRummyCard>(command);
        PlayerHand1 = new HandObservable<RegularRummyCard>(command);
        TempSets = new(command, resolver);
        MainSets = new(command);
        TempSets.HowManySets = 8;
        MainSets.Text = "Main Sets";
    }
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard> TempSets;
    public MainSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard, PhaseSet, SavedSet> MainSets;
    public DeckObservablePile<RegularRummyCard> Deck1 { get; set; }
    public SingleObservablePile<RegularRummyCard> Pile1 { get; set; }
    public HandObservable<RegularRummyCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularRummyCard>? OtherPile { get; set; }
}