namespace Skipbo.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SkipboVMData : IBasicCardGamesData<SkipboCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int CardsToShuffle { get; set; }
    public SkipboVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        StockPile = new(command);
        PublicPiles = new(command);
    }
    public DeckObservablePile<SkipboCardInformation> Deck1 { get; set; }
    public SingleObservablePile<SkipboCardInformation> Pile1 { get; set; }
    public HandObservable<SkipboCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<SkipboCardInformation>? OtherPile { get; set; }
    public StockViewModel StockPile;
    public DiscardPilesVM<SkipboCardInformation>? DiscardPiles;
    public PublicPilesViewModel PublicPiles;
}