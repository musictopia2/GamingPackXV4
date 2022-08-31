namespace YahtzeeHandsDown.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class YahtzeeHandsDownVMData : IBasicCardGamesData<YahtzeeHandsDownCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public YahtzeeHandsDownVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        ComboHandList = new(command)
        {
            Text = "Category Cards"
        };
        ChancePile = new(command)
        {
            Visible = false,
            CurrentOnly = true,
            Text = "Chance"
        };
    }
    public HandObservable<ComboCardInfo>? ComboHandList;
    public SingleObservablePile<ChanceCardInfo>? ChancePile;
    public DeckObservablePile<YahtzeeHandsDownCardInformation> Deck1 { get; set; }
    public SingleObservablePile<YahtzeeHandsDownCardInformation> Pile1 { get; set; }
    public HandObservable<YahtzeeHandsDownCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<YahtzeeHandsDownCardInformation>? OtherPile { get; set; }
}