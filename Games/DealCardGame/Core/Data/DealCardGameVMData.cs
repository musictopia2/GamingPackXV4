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
    [LabelColumn]
    public string Instructions { get; set; } = "";

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
        Properties = new(command);
        Properties.Text = "Properties To Pay With";
        Properties.AutoSelect = EnumHandAutoType.SelectOneOnly;
    }
    public DeckObservablePile<DealCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<DealCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation>? OtherPile { get; set; }
    public DealCardGameCardInformation? ShownCard { get; set; }
    public HandObservable<DealCardGameCardInformation> Bank { get; set; }
    public HandObservable<DealCardGameCardInformation> Payments { get; set; }
    public HandObservable<DealCardGameCardInformation> Properties { get; set; }
    public string ChosenPlayer { get; set; } = ""; //this is needed so you can see the cards that are needed.
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}