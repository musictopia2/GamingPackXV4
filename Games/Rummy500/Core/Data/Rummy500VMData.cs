namespace Rummy500.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class Rummy500VMData : IBasicCardGamesData<RegularRummyCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public Rummy500VMData(CommandContainer command)
    {
        Deck1 = new DeckObservablePile<RegularRummyCard>(command);
        Pile1 = new SingleObservablePile<RegularRummyCard>(command);
        PlayerHand1 = new HandObservable<RegularRummyCard>(command);
        MainSets1 = new(command);
        DiscardList1 = new DiscardListCP(command);
        Pile1.Visible = false;
    }
    public DeckObservablePile<RegularRummyCard> Deck1 { get; set; }
    public SingleObservablePile<RegularRummyCard> Pile1 { get; set; }
    public HandObservable<RegularRummyCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularRummyCard>? OtherPile { get; set; }
    public DiscardListCP DiscardList1;
    public MainSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard, RummySet, SavedSet> MainSets1;
}