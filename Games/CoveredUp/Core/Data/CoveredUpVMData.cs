namespace CoveredUp.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class CoveredUpVMData : IBasicCardGamesData<RegularSimpleCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int Round { get; set; }
    public CoveredUpVMData(CommandContainer command)
    {
        Deck1 = new DeckObservablePile<RegularSimpleCard>(command);
        Pile1 = new SingleObservablePile<RegularSimpleCard>(command);
        PlayerHand1 = new(command)
        {
            Visible = false //needs to be false
        };
        OtherPile = new(command)
        {
            CurrentOnly = true,
            Text = "Current"
        };
        OtherPile.FirstLoad(new());
    }
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
}