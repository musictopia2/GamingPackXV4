namespace SixtySix2Player.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SixtySix2PlayerVMData : ITrickCardGamesData<SixtySix2PlayerCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    [LabelColumn]
    public int BonusPoints { get; set; }
    [LabelColumn]
    public string DeckCount => Deck1.TextToAppear;
    public static BasicList<ScoreValuePair> GetDescriptionList()
    {
        return new()
        {
            new ScoreValuePair("Marriage In Trumps (K, Q announced)", 40),
            new ScoreValuePair("Marriage In Any Other Suit (K, Q announced)", 20),
            new ScoreValuePair("Each Ace", 11),
            new ScoreValuePair("Each 10", 10),
            new ScoreValuePair("Each King", 4),
            new ScoreValuePair("Each Queen", 3),
            new ScoreValuePair("Each Jack", 2),
            new ScoreValuePair("Last Trick", 10)
        };
    }
    public SixtySix2PlayerVMData(CommandContainer command,
            TwoPlayerTrickObservable<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        Marriage1 = new(command);
        Deck1.DrawInCenter = true;
        PlayerHand1.Maximum = 6;
        Marriage1.Visible = false;
        Marriage1.Text = "Cards For Marriage";
    }
    BasicTrickAreaObservable<EnumSuitList, SixtySix2PlayerCardInformation> ITrickCardGamesData<SixtySix2PlayerCardInformation, EnumSuitList>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (TwoPlayerTrickObservable<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>)value;
    }
    public HandObservable<SixtySix2PlayerCardInformation> Marriage1;
    public TwoPlayerTrickObservable<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo> TrickArea1 { get; set; }
    public DeckObservablePile<SixtySix2PlayerCardInformation> Deck1 { get; set; }
    public SingleObservablePile<SixtySix2PlayerCardInformation> Pile1 { get; set; }
    public HandObservable<SixtySix2PlayerCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<SixtySix2PlayerCardInformation>? OtherPile { get; set; }
}