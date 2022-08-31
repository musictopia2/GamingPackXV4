namespace Racko.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class RackoVMData : IBasicCardGamesData<RackoCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public RackoVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        OtherPile = new(command);
        OtherPile.Text = "Current Card";
        OtherPile.CurrentOnly = true;
        OtherPile.FirstLoad(new());
    }
    public DeckObservablePile<RackoCardInformation> Deck1 { get; set; }
    public SingleObservablePile<RackoCardInformation> Pile1 { get; set; }
    public HandObservable<RackoCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<RackoCardInformation>? OtherPile { get; set; }
}