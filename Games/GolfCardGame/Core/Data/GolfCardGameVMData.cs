namespace GolfCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class GolfCardGameVMData : IBasicCardGamesData<RegularSimpleCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int Round { get; set; }
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public GolfCardGameVMData(CommandContainer command, GolfCardGameGameContainer gameContainer)
    {
        Deck1 = new DeckObservablePile<RegularSimpleCard>(command);
        Pile1 = new SingleObservablePile<RegularSimpleCard>(command);
        PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
        OtherPile = new(command);
        OtherPile.CurrentOnly = true;
        OtherPile.Text = "Current";
        HiddenCards1 = new HiddenCards(gameContainer);
        Beginnings1 = new Beginnings(command);
        GolfHand1 = new GolfHand(gameContainer);
    }
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
    public HiddenCards HiddenCards1;
    public Beginnings Beginnings1;
    public GolfHand GolfHand1;
}