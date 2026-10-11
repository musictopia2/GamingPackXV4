namespace BasicMultiplayerTrickCardGames.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class BasicMultiplayerTrickCardGamesVMData(CommandContainer command,
        BasicTrickAreaObservable<EnumSuitList, BasicMultiplayerTrickCardGamesCardInformation> trickArea1
            ) : ITrickCardGamesData<BasicMultiplayerTrickCardGamesCardInformation, EnumSuitList>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial EnumSuitList TrumpSuit { get; set; }
    public BasicTrickAreaObservable<EnumSuitList, BasicMultiplayerTrickCardGamesCardInformation> TrickArea1 { get; set; } = trickArea1;
    public DeckObservablePile<BasicMultiplayerTrickCardGamesCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<BasicMultiplayerTrickCardGamesCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<BasicMultiplayerTrickCardGamesCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<BasicMultiplayerTrickCardGamesCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}