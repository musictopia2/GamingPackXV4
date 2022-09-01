namespace PickelCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class PickelCardGameVMData : ITrickCardGamesData<PickelCardGameCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    [LabelColumn]
    public int BidAmount { get; set; }
    public PickelCardGameVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, PickelCardGameCardInformation> trickArea1,
            IGamePackageResolver resolver
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        PlayerHand1.Text = "Your Cards";
        TrickArea1 = trickArea1;
        Bid1 = new(command, resolver);
        Suit1 = new(command, new SuitListChooser());
        Suit1.AutoSelectCategory = EnumAutoSelectCategory.AutoSelect;
    }
    public NumberPicker Bid1 { get; set; }
    public SimpleEnumPickerVM<EnumSuitList> Suit1 { get; set; }
    public BasicTrickAreaObservable<EnumSuitList, PickelCardGameCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<PickelCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<PickelCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<PickelCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<PickelCardGameCardInformation>? OtherPile { get; set; }
}