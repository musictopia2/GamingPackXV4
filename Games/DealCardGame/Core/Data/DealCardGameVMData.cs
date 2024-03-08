namespace DealCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class DealCardGameVMData : IBasicCardGamesData<DealCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public DealCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
    }
    public DeckObservablePile<DealCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<DealCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation>? OtherPile { get; set; }

    public DealCardGameCardInformation? ShownCard { get; set; }

    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}