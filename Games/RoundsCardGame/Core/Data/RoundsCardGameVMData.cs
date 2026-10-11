namespace RoundsCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class RoundsCardGameVMData(CommandContainer command,
        BasicTrickAreaObservable<EnumSuitList, RoundsCardGameCardInformation> trickArea1
            ) : ITrickCardGamesData<RoundsCardGameCardInformation, EnumSuitList>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial EnumSuitList TrumpSuit { get; set; }
    public BasicTrickAreaObservable<EnumSuitList, RoundsCardGameCardInformation> TrickArea1 { get; set; } = trickArea1;
    public DeckObservablePile<RoundsCardGameCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<RoundsCardGameCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<RoundsCardGameCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<RoundsCardGameCardInformation>? OtherPile { get; set; }
}