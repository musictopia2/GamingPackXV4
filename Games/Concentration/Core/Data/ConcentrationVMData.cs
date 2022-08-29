namespace Concentration.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class ConcentrationVMData : IBasicCardGamesData<RegularSimpleCard>
{
    public GameBoardClass GameBoard1;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public ConcentrationVMData(CommandContainer command, ConcentrationGameContainer gameContainer)
    {
        Deck1 = new DeckObservablePile<RegularSimpleCard>(command);
        Pile1 = new SingleObservablePile<RegularSimpleCard>(command);
        PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
        GameBoard1 = new GameBoardClass(gameContainer);
    }
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
}