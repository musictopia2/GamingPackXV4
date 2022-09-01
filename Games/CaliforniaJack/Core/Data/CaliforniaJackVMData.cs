namespace CaliforniaJack.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class CaliforniaJackVMData : ITrickCardGamesData<CaliforniaJackCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public CaliforniaJackVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, CaliforniaJackCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
    }
    public BasicTrickAreaObservable<EnumSuitList, CaliforniaJackCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<CaliforniaJackCardInformation> Deck1 { get; set; }
    public SingleObservablePile<CaliforniaJackCardInformation> Pile1 { get; set; }
    public HandObservable<CaliforniaJackCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<CaliforniaJackCardInformation>? OtherPile { get; set; }
}