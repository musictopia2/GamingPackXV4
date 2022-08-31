namespace DutchBlitz.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class DutchBlitzVMData : IBasicCardGamesData<DutchBlitzCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public DutchBlitzVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
    }
    public DeckObservablePile<DutchBlitzCardInformation> Deck1 { get; set; }
    public SingleObservablePile<DutchBlitzCardInformation> Pile1 { get; set; }
    public HandObservable<DutchBlitzCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<DutchBlitzCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}