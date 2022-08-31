namespace SorryCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SorryCardGameVMData : IBasicCardGamesData<SorryCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int UpTo { get; set; }
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public SorryCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        OtherPile = new(command);
        OtherPile.Text = "Play Pile";
        OtherPile.FirstLoad(new SorryCardGameCardInformation());
    }
    public DeckObservablePile<SorryCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<SorryCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<SorryCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<SorryCardGameCardInformation>? OtherPile { get; set; }
}