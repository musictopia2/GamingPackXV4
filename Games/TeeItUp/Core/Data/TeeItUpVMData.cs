namespace TeeItUp.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class TeeItUpVMData : IBasicCardGamesData<TeeItUpCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int Round { get; set; }
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public TeeItUpVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        OtherPile = new(command);
        OtherPile.CurrentOnly = true;
        OtherPile.Text = "Current Card";
        OtherPile.FirstLoad(new TeeItUpCardInformation());
        PlayerHand1.Visible = false; //try this too.
    }
    public DeckObservablePile<TeeItUpCardInformation> Deck1 { get; set; }
    public SingleObservablePile<TeeItUpCardInformation> Pile1 { get; set; }
    public HandObservable<TeeItUpCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<TeeItUpCardInformation>? OtherPile { get; set; }
}