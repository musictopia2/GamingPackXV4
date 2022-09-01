namespace LifeCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class LifeCardGameVMData : IBasicCardGamesData<LifeCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string OtherText { get; set; } = "";
    public LifeCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        CurrentPile = new(command);
        CurrentPile.Text = "Current Card";
        OtherPile = CurrentPile;
    }
    public SingleObservablePile<LifeCardGameCardInformation> CurrentPile;
    public DeckObservablePile<LifeCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<LifeCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<LifeCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<LifeCardGameCardInformation>? OtherPile { get; set; }
}