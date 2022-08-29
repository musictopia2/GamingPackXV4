namespace CrazyEights.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class CrazyEightsVMData : IBasicCardGamesData<RegularSimpleCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public bool ChooseSuit { get; set; }
    public SimpleEnumPickerVM<EnumSuitList> SuitChooser;
    public CrazyEightsVMData(CommandContainer command)
    {
        Deck1 = new DeckObservablePile<RegularSimpleCard>(command);
        Pile1 = new SingleObservablePile<RegularSimpleCard>(command);
        PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
        SuitChooser = new(command, new SuitListChooser());
        SuitChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
    }
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
}