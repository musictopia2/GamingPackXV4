namespace DummyRummy.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class DummyRummyVMData : IBasicCardGamesData<RegularRummyCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int UpTo { get; set; }
    public DummyRummyVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new DeckObservablePile<RegularRummyCard>(command);
        Pile1 = new SingleObservablePile<RegularRummyCard>(command);
        PlayerHand1 = new(command);
        PlayerHand1.Maximum = 14; //do it this way to stop the problem with jumping around.
        TempSets = new(command, resolver);
        MainSets = new(command);
        TempSets.HowManySets = 6;
    }
    public DeckObservablePile<RegularRummyCard> Deck1 { get; set; }
    public SingleObservablePile<RegularRummyCard> Pile1 { get; set; }
    public HandObservable<RegularRummyCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularRummyCard>? OtherPile { get; set; }
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard> TempSets;
    public MainSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard, DummySet, SavedSet> MainSets;
}