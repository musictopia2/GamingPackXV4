namespace BasicMultiplayerMiscCardGames.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class BasicMultiplayerMiscCardGamesVMData : IBasicCardGamesData<BasicMultiplayerMiscCardGamesCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public BasicMultiplayerMiscCardGamesVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
    }
    public DeckObservablePile<BasicMultiplayerMiscCardGamesCardInformation> Deck1 { get; set; }
    public SingleObservablePile<BasicMultiplayerMiscCardGamesCardInformation> Pile1 { get; set; }
    public HandObservable<BasicMultiplayerMiscCardGamesCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<BasicMultiplayerMiscCardGamesCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}