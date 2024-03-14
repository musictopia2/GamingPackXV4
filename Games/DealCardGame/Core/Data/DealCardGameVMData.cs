using System.Xml.Linq;

namespace DealCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class DealCardGameVMData : IBasicCardGamesData<DealCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string OtherTurn { get; set; } = "";

    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";

    //another ui can use this.
    [LabelColumn]
    public decimal Owed { get; set; }
    [LabelColumn]
    public decimal PaidSoFar { get; set; }
    public DealCardGameVMData(CommandContainer command, 
        DealCardGameGameContainer gameContainer, IToast toast, PrivateAutoResumeProcesses privateAutoResume)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        ReceivedPayments = new(command);
        ReceivedPayments.Text = "Payments Received";
        ReceivedPayments.AutoSelect = EnumHandAutoType.ShowObjectOnly; //i think.
        StolenCards = new(command);
        StolenCards.Text = "Cards From Stolen Set";
        StolenCards.AutoSelect = EnumHandAutoType.ShowObjectOnly;
        PlayerPicker = new(command, gameContainer.Resolver);
        PlayerPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
        PlayerPicker.ItemSelectedAsync = ChosePlayerAsync;
        PlayerPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased; //because you have the name.
        Payments = new(gameContainer.Command);
        Payments.Text = "Payments";
        Payments.AutoSelect = EnumHandAutoType.ShowObjectOnly; //show only.
        Bank = new(gameContainer.Command);
        Bank.Text = "Bank";
        Bank.AutoSelect = EnumHandAutoType.SelectAsMany;
        Bank.HandList.Sort();
        Properties = new(gameContainer.Command);
        Properties.Text = "Properties To Pay With";
        Properties.AutoSelect = EnumHandAutoType.SelectOneOnly;
        YourCompleteSets = new(gameContainer, toast, privateAutoResume);
    }
    public DeckObservablePile<DealCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<DealCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<DealCardGameCardInformation>? OtherPile { get; set; }
    public DealCardGameCardInformation? ShownCard { get; set; }
    public HandObservable<DealCardGameCardInformation> StolenCards { get; set; }
    public string ChosenPlayer { get; set; } = ""; //this is needed so you can see the cards that are needed.
    public HandObservable<DealCardGameCardInformation> ReceivedPayments { get; set; }

    public HandObservable<DealCardGameCardInformation> Properties { get; set; }
    public HandObservable<DealCardGameCardInformation> Payments { get; set; }
    public HandObservable<DealCardGameCardInformation> Bank { get; set; }

    public PersonalCompleteSets YourCompleteSets { get; set; }
    public ListViewPicker PlayerPicker { get; set; }
    public TradeDisplayModel? TradeDisplay { get; set; }
    private Task ChosePlayerAsync(int index, string player)
    {
        PlayerPicker.SelectedIndex = index;
        return Task.CompletedTask;
    }

    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}