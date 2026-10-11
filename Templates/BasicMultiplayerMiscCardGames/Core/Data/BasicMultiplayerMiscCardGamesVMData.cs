namespace BasicMultiplayerMiscCardGames.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class BasicMultiplayerMiscCardGamesVMData(CommandContainer command) : IBasicCardGamesData<BasicMultiplayerMiscCardGamesCardInformation>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    public DeckObservablePile<BasicMultiplayerMiscCardGamesCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<BasicMultiplayerMiscCardGamesCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<BasicMultiplayerMiscCardGamesCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<BasicMultiplayerMiscCardGamesCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}