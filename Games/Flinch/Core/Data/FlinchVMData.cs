namespace Flinch.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class FlinchVMData(CommandContainer command) : IBasicCardGamesData<FlinchCardInformation>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int CardsToShuffle { get; set; }

    public StockViewModel StockPile = new(command);
    public DiscardPilesVM<FlinchCardInformation>? DiscardPiles;
    public PublicPilesViewModel PublicPiles = new(command);
    public DeckObservablePile<FlinchCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<FlinchCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<FlinchCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<FlinchCardInformation>? OtherPile { get; set; }
}