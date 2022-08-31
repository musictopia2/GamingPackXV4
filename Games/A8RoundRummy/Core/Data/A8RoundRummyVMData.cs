namespace A8RoundRummy.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class A8RoundRummyVMData : IBasicCardGamesData<A8RoundRummyCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string NextTurn { get; set; } = "";
    public A8RoundRummyVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        PlayerHand1.Maximum = 8;
    }
    public DeckObservablePile<A8RoundRummyCardInformation> Deck1 { get; set; }
    public SingleObservablePile<A8RoundRummyCardInformation> Pile1 { get; set; }
    public HandObservable<A8RoundRummyCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<A8RoundRummyCardInformation>? OtherPile { get; set; }
}