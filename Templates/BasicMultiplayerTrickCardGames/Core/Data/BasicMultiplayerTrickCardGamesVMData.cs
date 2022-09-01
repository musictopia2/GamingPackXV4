namespace BasicMultiplayerTrickCardGames.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class BasicMultiplayerTrickCardGamesVMData : ITrickCardGamesData<BasicMultiplayerTrickCardGamesCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public BasicMultiplayerTrickCardGamesVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, BasicMultiplayerTrickCardGamesCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
    }
    public BasicTrickAreaObservable<EnumSuitList, BasicMultiplayerTrickCardGamesCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<BasicMultiplayerTrickCardGamesCardInformation> Deck1 { get; set; }
    public SingleObservablePile<BasicMultiplayerTrickCardGamesCardInformation> Pile1 { get; set; }
    public HandObservable<BasicMultiplayerTrickCardGamesCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<BasicMultiplayerTrickCardGamesCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}