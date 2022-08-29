namespace GoFish.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class GoFishVMData : IBasicCardGamesData<RegularSimpleCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public GoFishVMData(CommandContainer command)
    {
        Deck1 = new DeckObservablePile<RegularSimpleCard>(command);
        Pile1 = new SingleObservablePile<RegularSimpleCard>(command);
        PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
        AskList = new(command);
        AskList.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent; //i think.
    }
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
    public GoFishChooserCP AskList;
    public EnumRegularCardValueList CardYouAsked { get; set; }
}