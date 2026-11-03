namespace Concentration.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class ConcentrationVMData(CommandContainer command, ConcentrationGameContainer gameContainer) : IBasicCardGamesData<RegularSimpleCard>
{
    public GameBoardClass GameBoard1 = new(gameContainer);
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; } = new DeckObservablePile<RegularSimpleCard>(command);
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; } = new SingleObservablePile<RegularSimpleCard>(command);
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; } = new HandObservable<RegularSimpleCard>(command);
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
}