namespace HitTheDeck.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class HitTheDeckVMData(CommandContainer command) : IBasicCardGamesData<HitTheDeckCardInformation>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial string NextPlayer { get; set; } = "";
    public DeckObservablePile<HitTheDeckCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<HitTheDeckCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<HitTheDeckCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<HitTheDeckCardInformation>? OtherPile { get; set; }
}