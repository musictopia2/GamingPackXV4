namespace MilkRun.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class MilkRunVMData(CommandContainer command) : IBasicCardGamesData<MilkRunCardInformation>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    public DeckObservablePile<MilkRunCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<MilkRunCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<MilkRunCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<MilkRunCardInformation>? OtherPile { get; set; }
}