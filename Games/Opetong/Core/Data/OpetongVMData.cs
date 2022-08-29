namespace Opetong.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class OpetongVMData : IBasicCardGamesData<RegularRummyCard>
{
    public OpetongVMData(CommandContainer command, IGamePackageResolver resolver, OpetongGameContainer gameContainer)
    {
        Deck1 = new DeckObservablePile<RegularRummyCard>(command);
        Pile1 = new SingleObservablePile<RegularRummyCard>(command);
        PlayerHand1 = new HandObservable<RegularRummyCard>(command);
        TempSets = new(command, resolver);
        TempSets.HowManySets = 3;
        MainSets = new MainSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard, RummySet, SavedSet>(command);
        Pool1 = new CardPool(gameContainer);
    }
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard> TempSets;
    public MainSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard, RummySet, SavedSet> MainSets;
    public CardPool Pool1;
    public DeckObservablePile<RegularRummyCard> Deck1 { get; set; }
    public SingleObservablePile<RegularRummyCard> Pile1 { get; set; }
    public HandObservable<RegularRummyCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularRummyCard>? OtherPile { get; set; }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
}