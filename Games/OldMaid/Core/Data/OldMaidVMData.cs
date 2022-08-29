namespace OldMaid.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class OldMaidVMData : IBasicCardGamesData<RegularSimpleCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public OldMaidVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        OpponentCards1 = new(command)
        {
            Text = "Opponent Cards",
            AutoSelect = EnumHandAutoType.None
        };
    }
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
    public HandObservable<RegularSimpleCard> OpponentCards1 { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}