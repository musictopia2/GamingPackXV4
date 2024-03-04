namespace ClueCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class ClueCardGameVMData : IBasicCardGamesData<ClueCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";

    [LabelColumn]
    public string FirstName { get; set; } = "";
    [LabelColumn]
    public string SecondName { get; set; } = "";
    public ClueCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        Pile1.Text = "Clue";
        Prediction = new(command);
        Prediction.Text = "Prediction";
        Prediction.Maximum = 2;
        Accusation = new(command);
        Accusation.Maximum = 3;
        Accusation.Visible = false;
        Accusation.Text = "Accusation";
    }
    public DeckObservablePile<ClueCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<ClueCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<ClueCardGameCardInformation> PlayerHand1 { get; set; }
    public HandObservable<ClueCardGameCardInformation> Prediction { get; set; }
    public HandObservable<ClueCardGameCardInformation> Accusation { get; set; }
    public SingleObservablePile<ClueCardGameCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}