namespace FourSuitRummy.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class FourSuitRummyVMData : IBasicCardGamesData<RegularRummyCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public FourSuitRummyVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new DeckObservablePile<RegularRummyCard>(command);
        Pile1 = new SingleObservablePile<RegularRummyCard>(command);
        PlayerHand1 = new HandObservable<RegularRummyCard>(command);
        TempSets = new(command, resolver)
        {
            HowManySets = 6
        };
    }
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, RegularRummyCard> TempSets;
    public DeckObservablePile<RegularRummyCard> Deck1 { get; set; }
    public SingleObservablePile<RegularRummyCard> Pile1 { get; set; }
    public HandObservable<RegularRummyCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularRummyCard>? OtherPile { get; set; }
}