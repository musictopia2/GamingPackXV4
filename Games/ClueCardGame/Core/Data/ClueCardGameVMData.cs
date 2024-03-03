namespace ClueCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class ClueCardGameVMData : IBasicCardGamesData<ClueCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public ClueCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
    }
    public DeckObservablePile<ClueCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<ClueCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<ClueCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<ClueCardGameCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}