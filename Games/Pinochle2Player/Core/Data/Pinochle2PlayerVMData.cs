namespace Pinochle2Player.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class Pinochle2PlayerVMData : ITrickCardGamesData<Pinochle2PlayerCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    [LabelColumn]
    public string DeckCount => Deck1.TextToAppear;
    public Pinochle2PlayerVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, Pinochle2PlayerCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        Guide1 = new();
        YourMelds = new(command);
        OpponentMelds = new(command);
        Deck1.DrawInCenter = true;
        YourMelds.Text = "Yours";
        OpponentMelds.Text = "Opponents";
        YourMelds.AutoSelect = EnumHandAutoType.SelectOneOnly;
    }
    public BasicTrickAreaObservable<EnumSuitList, Pinochle2PlayerCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<Pinochle2PlayerCardInformation> Deck1 { get; set; }
    public SingleObservablePile<Pinochle2PlayerCardInformation> Pile1 { get; set; }
    public HandObservable<Pinochle2PlayerCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<Pinochle2PlayerCardInformation>? OtherPile { get; set; }
    public ScoreGuideViewModel Guide1;
    public HandObservable<Pinochle2PlayerCardInformation> YourMelds;
    public HandObservable<Pinochle2PlayerCardInformation> OpponentMelds;
}