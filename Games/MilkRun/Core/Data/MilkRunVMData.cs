namespace MilkRun.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class MilkRunVMData : IBasicCardGamesData<MilkRunCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public MilkRunVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
    }
    public DeckObservablePile<MilkRunCardInformation> Deck1 { get; set; }
    public SingleObservablePile<MilkRunCardInformation> Pile1 { get; set; }
    public HandObservable<MilkRunCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<MilkRunCardInformation>? OtherPile { get; set; }
}