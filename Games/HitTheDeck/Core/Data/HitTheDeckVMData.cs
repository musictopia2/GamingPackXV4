namespace HitTheDeck.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class HitTheDeckVMData : IBasicCardGamesData<HitTheDeckCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string NextPlayer { get; set; } = "";
    public HitTheDeckVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
    }
    public DeckObservablePile<HitTheDeckCardInformation> Deck1 { get; set; }
    public SingleObservablePile<HitTheDeckCardInformation> Pile1 { get; set; }
    public HandObservable<HitTheDeckCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<HitTheDeckCardInformation>? OtherPile { get; set; }
}