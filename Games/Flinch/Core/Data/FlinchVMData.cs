namespace Flinch.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class FlinchVMData : IBasicCardGamesData<FlinchCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int CardsToShuffle { get; set; }
    public FlinchVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        StockPile = new(command);
        PublicPiles = new(command);
    }
    public StockViewModel StockPile;
    public DiscardPilesVM<FlinchCardInformation>? DiscardPiles;
    public PublicPilesViewModel PublicPiles;
    public DeckObservablePile<FlinchCardInformation> Deck1 { get; set; }
    public SingleObservablePile<FlinchCardInformation> Pile1 { get; set; }
    public HandObservable<FlinchCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<FlinchCardInformation>? OtherPile { get; set; }
}