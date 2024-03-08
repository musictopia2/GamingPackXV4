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

    //another ui can use this.
    [LabelColumn]
    public decimal Owed { get; set; }
    [LabelColumn]
    public decimal PaidSoFar { get; set; }
    public DealCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        Bank = new(command);
        Bank.Text = "Bank";
        Bank.AutoSelect = EnumHandAutoType.SelectAsMany;
        Payments = new(command);
        Payments.Text = "Payments";
        Payments.AutoSelect = EnumHandAutoType.ShowObjectOnly; //show only.
    }
    public DeckObservablePile<DealCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<DealCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation>? OtherPile { get; set; }
    public DealCardGameCardInformation? ShownCard { get; set; }
    public HandObservable<DealCardGameCardInformation> Bank { get; set; }
    public HandObservable<DealCardGameCardInformation> Payments { get; set; }

    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}