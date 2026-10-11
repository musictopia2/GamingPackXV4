namespace CaliforniaJack.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class CaliforniaJackVMData(CommandContainer command,
        BasicTrickAreaObservable<EnumSuitList, CaliforniaJackCardInformation> trickArea1
            ) : ITrickCardGamesData<CaliforniaJackCardInformation, EnumSuitList>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial EnumSuitList TrumpSuit { get; set; }
    public BasicTrickAreaObservable<EnumSuitList, CaliforniaJackCardInformation> TrickArea1 { get; set; } = trickArea1;
    public DeckObservablePile<CaliforniaJackCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<CaliforniaJackCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<CaliforniaJackCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<CaliforniaJackCardInformation>? OtherPile { get; set; }
}