namespace Skipbo.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SkipboVMData(CommandContainer command) : IBasicCardGamesData<SkipboCardInformation>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int CardsToShuffle { get; set; }
    public DeckObservablePile<SkipboCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<SkipboCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<SkipboCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<SkipboCardInformation>? OtherPile { get; set; }
    public StockViewModel StockPile = new(command);
    public DiscardPilesVM<SkipboCardInformation>? DiscardPiles;
    public PublicPilesViewModel PublicPiles = new(command);
}