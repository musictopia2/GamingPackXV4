namespace RoundsCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class RoundsCardGameVMData : ITrickCardGamesData<RoundsCardGameCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public RoundsCardGameVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, RoundsCardGameCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
    }
    public BasicTrickAreaObservable<EnumSuitList, RoundsCardGameCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<RoundsCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<RoundsCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<RoundsCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<RoundsCardGameCardInformation>? OtherPile { get; set; }
}