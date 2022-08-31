namespace MonopolyCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class MonopolyCardGameVMData : IBasicCardGamesData<MonopolyCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public MonopolyCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TempHand1 = new(command);
        AdditionalInfo1 = new();
    }
    public DetailCardViewModel AdditionalInfo1;
    public DeckObservablePile<MonopolyCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<MonopolyCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<MonopolyCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<MonopolyCardGameCardInformation>? OtherPile { get; set; }
    public HandObservable<MonopolyCardGameCardInformation> TempHand1 { get; set; }
}